using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krawlr.Core
{
    public interface IUrlQueueService
    {
        void Add(string url);
        string Dequeue();
        bool Peek();
    }

    public class UrlQueueService : IUrlQueueService
    {
        static ConcurrentQueue<string> Queue = new ConcurrentQueue<string>();
        static HashSet<string> List = new HashSet<string>();

        protected string _baseUrl;

        public UrlQueueService(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public void Add(string url)
        {
            if (url.StartsWith(_baseUrl) == false)
                return;

            var uri = new Uri(url);
            var path = uri.LocalPath;

            if (List.Contains(path)) // TODO: Lock
                return;

            List.Add(path); // TODO: Add Lock
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
