using System;
using System.Collections.Generic;
using System.Linq;
using Krawlr.Core.Extensions;
using Krawlr.Core.PageActions;

namespace Krawlr.Core
{
    public class Application
    {
        protected Page _page;
        protected IUrlQueueService _queueService;
        protected IEnumerable<IPageAction> _pageActions;

        public Application(Page page, IUrlQueueService urlQueueService, IEnumerable<IPageAction> pageActions)
        {
            _page = page;
            _queueService = urlQueueService;
            _pageActions = pageActions;
        }

        public void Start()
        {
            //var links = page.Links()
            //    .Select(el => el.GetAttribute("href")).Distinct()
            //    .Where(t => t.StartsWith(baseUrl, StringComparison.InvariantCultureIgnoreCase));

            //links.ToList().ForEach(l => queueService.Add(l));

            while (_queueService.Peek())
            {
                // Load
                var url = _queueService.Dequeue();
                _page.NavigateToViewWithJsErrorProxy(url);

                // Log
                // TODO
                System.Console.WriteLine(String.Format("Navigating to {0}", url));

                // Actions to perform on this URL?
                _pageActions
                    .Where(a => url.ContainsEx(a.Url)).ToList()
                    .ForEach(pa => pa.Invoke(_page.Driver));

                // Links?
                var links = _page.Links()
                    .Select(el => el.GetAttribute("href")).Distinct()
                    .Where(t => t != null)
                    .Where(t => t.StartsWith(_queueService.BaseUrl, StringComparison.InvariantCultureIgnoreCase));
                links.ToList().ForEach(l => _queueService.Add(l));
            }
        }
    }
}
