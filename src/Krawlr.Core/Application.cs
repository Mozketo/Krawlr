using System;
using System.Linq;
using Krawlr.Core.Extensions;

namespace Krawlr.Core
{
    public class Application
    {
        protected Page _page;
        protected IUrlQueueService _queueService;
        protected IConfiguration _configuration;
        protected IPageActionService _pageActionService;

        public Application(Page page, 
            IUrlQueueService urlQueueService, 
            IConfiguration configuration,
            IPageActionService pageActionService)
        {
            _page = page;
            _queueService = urlQueueService;
            _configuration = configuration;
            _pageActionService = pageActionService;

            _queueService.Progress += (sender, args) =>
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(String.Format("{0} pages remaining", args.Remaining));
                Console.ResetColor();
            };
        }

        public void Start()
        {
            if (_configuration.Inclusions.Any())
                _configuration.Inclusions.Iter(i => _queueService.Add(i));

            while (_queueService.Peek())
            {
                // Load
                var timer = System.Diagnostics.Stopwatch.StartNew();
                var url = _queueService.Dequeue();
                _page.NavigateToViewWithJsErrorProxy(url);
                TimeSpan timeTaken = timer.Elapsed;

                // Page errors?
                var errors = _page.GetJavaScriptErrors(TimeSpan.FromMinutes(1));

                // Log
                Console.WriteLine(String.Format("Navigating in {0} ms to {1} with status code of {2}. JS Errors? {3}",
                    timeTaken.TotalMilliseconds,
                    url,
                    _page.ReponseCode,
                    (errors.Any()) ? "Yes" : "No"));

                // Actions to perform on this URL?
                _pageActionService.GenerateInstances(url);
                //_pageActions
                //    .Where(a => url.ContainsEx(a.Url)).ToList()
                //    .ForEach(pa => pa.Invoke(_page.Driver));

                // Links
                var links = _page.Links()
                    .Select(el => el.GetAttribute("href")).Distinct();
                links.ToList().ForEach(l => _queueService.Add(l));
            }
        }
    }
}
