using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Krawlr.Core.Extensions;

namespace Krawlr.Core.Services
{
    public interface IUrlQueueService
    {
        event ProgressEventHandler Progress;
        void Add(string url);
        string Dequeue();
        bool Peek();
    }

    public class UrlQueueService : IUrlQueueService
    {
        static ConcurrentQueue<string> Queue = new ConcurrentQueue<string>();
        static HashSet<string> List = new HashSet<string>();

        public event ProgressEventHandler Progress;

        IConfiguration _options;

        public UrlQueueService(IConfiguration options)
        {
            _options = options;
        }

        public void Add(string url)
        {
            if (!url.HasValue() || url.StartsWith(_options.BaseUrl) == false)
                return;

            var uri = new Uri(url);
            var path = uri.LocalPath.RemoveTrailing('/');

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
            Queue.Enqueue(url);
        }

        public string Dequeue()
        {
            if (Progress != null)
                Progress(this, new ProgressEventArgs { Remaining = Queue.Count, Count = List.Count() });

            string result;
            Queue.TryDequeue(out result);
            return result;
        }

        public bool Peek()
        {
            string result;
            return Queue.TryPeek(out result);
        }
    }

    public class ProgressEventArgs : EventArgs
    {
        public int Remaining { get; set; }
        public int Count { get; set; }
    }

    public delegate void ProgressEventHandler(object sender, ProgressEventArgs e);
}
