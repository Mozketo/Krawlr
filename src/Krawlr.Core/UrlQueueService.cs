using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Krawlr.Core.Extensions;

namespace Krawlr.Core
{
    public interface IUrlQueueService
    {
        string BaseUrl { get; }
        void Add(string url);
        string Dequeue();
        bool Peek();
    }

    public class UrlQueueService : IUrlQueueService
    {
        static ConcurrentQueue<string> Queue = new ConcurrentQueue<string>();
        static HashSet<string> List = new HashSet<string>();

        public string BaseUrl { get; protected set; }

        public UrlQueueService(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

        public void Add(string url)
        {
            if (url.StartsWith(BaseUrl) == false)
                return;

            var uri = new Uri(url);
            var path = uri.LocalPath.RemoveTrailing('/');

            if (List.Contains(path)) // TODO: Lock?
                return;

            List.Add(path); // TODO: Add Lock?
            Queue.Enqueue(url);
        }

        public string Dequeue()
        {
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
}
