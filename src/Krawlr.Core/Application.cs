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
        protected IOutputService _outputService;

        public Application(Page page,
            IUrlQueueService urlQueueService,
            IConfiguration configuration,
            IPageActionService pageActionService,
            IOutputService outputService)
        {
            _page = page;
            _queueService = urlQueueService;
            _configuration = configuration;
            _pageActionService = pageActionService;
            _outputService = outputService;

            if (!_configuration.Silent)
            {
                _queueService.Progress += (sender, args) =>
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(String.Format("{0} pages remaining", args.Remaining));
                    Console.ResetColor();
                };
            }
        }

        public void Start()
        {
            if (_configuration.Inclusions.Any())
                _configuration.Inclusions.Iter(i => _queueService.Add(i));

            while (_queueService.Peek())
            {
                var response = new Response();
                var timer = System.Diagnostics.Stopwatch.StartNew();

                // Load
                response.Url = _queueService.Dequeue();
                _page.NavigateToViewWithJsErrorProxy(response.Url);
                response.TimeTakenMs = timer.ElapsedMilliseconds;

                // Page errors?
                response.JavascriptErrors = _page.GetJavaScriptErrors(TimeSpan.FromMinutes(1));
                response.Code = _page.ReponseCode;

                // Log
                _outputService.Write(response);

                // Selenium scripts for this URL
                _pageActionService.Invoke(response.Url);

                // Links
                var links = _page.Links()
                    .Select(el => el.GetAttribute("href")).Distinct();
                links.ToList().ForEach(l => _queueService.Add(l));
            }
        }
    }
}
