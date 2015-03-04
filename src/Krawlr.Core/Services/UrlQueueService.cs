using System;
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
        bool Peek { get; }
        int QueueSize { get; }
        void Add(string url);
        void Add(IEnumerable<string> urls);
    }

    public class UrlQueueService : IUrlQueueService
    {
        const string _guid = "guid";

        public event ProgressEventHandler Progress;
        static HashSet<string> List = new HashSet<string>();
        static ConcurrentDictionary<string, bool> BusMirror = new ConcurrentDictionary<string, bool>();

        protected IConfiguration _options;
        protected IMessageService _mQServer;
        protected IWriterService _writer;

        public bool Peek { get { return BusMirror.Values.Any(v => v == false); } }
        public int QueueSize { get { return BusMirror.Values.Count(v => v == false); } }

        public UrlQueueService(IConfiguration options, IMessageService messageService, IWriterService writer)
        {
            _options = options;
            _mQServer = messageService;
            _writer = writer;
        }

        public void Add(string url)
        {
            if (!url.HasValue())
                return;

            // If relative URL assume that it's for the current site.
            if (url.IndexOf('/') == 0)
                url = $"{_options.BaseUrl.RemoveTrailing('/')}{url}";

            if (url.StartsWith(_options.BaseUrl) == false)
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
                BusMirror.TryAdd(url, false);
                mqClient.Publish(new Url { Path = url, });

                // Now wait for the response
                IMessage<UrlResponse> responseMsg = mqClient.Get<UrlResponse>(QueueNames<UrlResponse>.In, TimeSpan.FromSeconds(90));
                mqClient.Ack(responseMsg);
                var response = responseMsg.GetBody();

                // Log response
                _writer.Write(response.Response);

                // Parse for new links
                Add(response.Links);

                // Update the BusMirror to reflect processing is complete
                BusMirror.TryUpdate(url, true, false);
                if (Progress != null)
                    Progress(this, new ProgressEventArgs { Count = BusMirror.Count, Remaining = this.QueueSize });
            }
        }
    }

    public class ProgressEventArgs : EventArgs
    {
        public int Remaining { get; set; }
        public int Count { get; set; }
    }

    public delegate void ProgressEventHandler(object sender, ProgressEventArgs e);
}
