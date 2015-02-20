﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MZMemoize.Extensions;
using ServiceStack.Messaging;
using Krawlr.Core.Extensions;
using Krawlr.Core.DTO;
using ServiceStack;

namespace Krawlr.Core.Services
{
    public interface IUrlQueueService
    {
        event ProgressEventHandler Progress;
        void Add(string url);
        void Add(IEnumerable<string> urls);
        bool TryDequeue(out string url);
        bool TryPeek(out string url);
        bool Peek();
    }

    public class UrlQueueService : IUrlQueueService
    {
        const string _guid = "guid";

        static ConcurrentQueue<string> Queue = new ConcurrentQueue<string>();
        static HashSet<string> List = new HashSet<string>();

        public event ProgressEventHandler Progress;

        protected IConfiguration _options;
        protected IMessageService _mQServer;
        protected IWriterService _writer;

        public UrlQueueService(IConfiguration options, IMessageService messageService, IWriterService writer)
        {
            _options = options;
            _mQServer = messageService;
            _writer = writer;
        }

        public void Add(string url)
        {
            // If relative URL assume that it's for the current site.
            if (url.IndexOf('/') == 0)
                url = $"{_options.BaseUrl.RemoveTrailing('/')}{url}";

            if (!url.HasValue() || url.StartsWith(_options.BaseUrl) == false)
                return;

            var uri = new Uri(url);
            var path = $"{uri.LocalPath}{uri.Fragment}".RemoveTrailing('#').RemoveTrailing('/');

            // Experimental, deconstruct the URL, remove GUIDs, reconstruct URL.
            // In this scenario it would be required to treat the URL as /route/{guid} yet be stored for lookup
            // as /route/guid so that later matches are made against a common string. It's getting messy
            // As an efficency step only do this for URLs that contain numbers, chances these are IDs
            if (_options.IgnoreGuids && path.Any(char.IsDigit))
            {
                path = String.Join("/", path.Split('/').Select(p =>
                {
                    Guid guid;
                    return Guid.TryParse(p, out guid) ? _guid : p;
                }));
            }

            if (List.Contains(path)) // TODO: Lock?
                return;

            // Is this path in the list of exclusions? If so the URL should be skipped. Example /logout might need to be ignored
            // otherwise the crawler may leave the crawl early.
            bool isExcluded = _options.Exclusions.Any(e => url.ContainsEx(e));
            if (isExcluded)
                return;

            if (_options.MaxPageLinksToFollow > 0 && List.Count() > _options.MaxPageLinksToFollow)
                return;

            List.Add(path); // TODO: Add Lock?
            //Queue.Enqueue(url);
            Publish(url);
        }

        public void Add(IEnumerable<string> urls)
        {
            urls.Iter(url => Add(url));
        }

        protected void Publish(string url)
        {
            using (var mqClient = _mQServer.CreateMessageQueueClient())
            {
                mqClient.Publish(new Url { Path = url, });

                // Now wait for the response
                IMessage<UrlResponse> responseMsg = mqClient.Get<UrlResponse>(QueueNames<UrlResponse>.In, TimeSpan.FromSeconds(90));
                mqClient.Ack(responseMsg);
                var response = responseMsg.GetBody();

                // Log response
                _writer.Write(response.Response);

                // Parse for new links
                Add(response.Links);
            }
        }

        public bool TryDequeue(out string url)
        {
            if (Progress != null)
                Progress(this, new ProgressEventArgs { Remaining = Queue.Count, Count = List.Count() });

            bool result = Queue.TryDequeue(out url);
            return result;
        }

        public bool TryPeek(out string url)
        {
            bool result = Queue.TryPeek(out url);
            return result;
        }

        public bool Peek()
        {
            string url;
            return Queue.TryPeek(out url);
        }
    }

    public class ProgressEventArgs : EventArgs
    {
        public int Remaining { get; set; }
        public int Count { get; set; }
    }

    public delegate void ProgressEventHandler(object sender, ProgressEventArgs e);
}
