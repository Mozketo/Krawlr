using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krawlr.Core
{
    using Funq;

    public class Application
    {
        protected Container _container;
        protected IUrlQueueService _queueService;

        public Application(Container container, IUrlQueueService urlQueueService)
        {
            _container = container;
            _queueService = urlQueueService;
        }

        public void Start()
        {
            var page = _container.Resolve<Page>();

            //var links = page.Links()
            //    .Select(el => el.GetAttribute("href")).Distinct()
            //    .Where(t => t.StartsWith(baseUrl, StringComparison.InvariantCultureIgnoreCase));

            //links.ToList().ForEach(l => queueService.Add(l));

            while (_queueService.Peek())
            {
                // Load
                var url = _queueService.Dequeue();
                page.NavigateToViewWithJsErrorProxy(url);

                // Log
                // TODO
                System.Console.WriteLine(String.Format("Navigating to {0}", url));

                // Links?
                var links = page.Links()
                    .Select(el => el.GetAttribute("href")).Distinct()
                    .Where(t => t.StartsWith(page.BaseUrl, StringComparison.InvariantCultureIgnoreCase));
                links.ToList().ForEach(l => _queueService.Add(l));
            }
        }
    }
}
