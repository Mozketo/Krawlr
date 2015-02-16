using System;
using System.Linq;
using Krawlr.Core.Extensions;
using Krawlr.Core.Services;

namespace Krawlr.Core
{
    public class Application
    {
        protected Page _page;
        protected IUrlQueueService _queueService;
        protected IConfiguration _configuration;
        protected IPageActionService _pageActionService;
        protected IOutputService _outputService;
        protected ILog _log;

        public Application(Page page,
            IUrlQueueService urlQueueService,
            IConfiguration configuration,
            IPageActionService pageActionService,
            IOutputService outputService,
            ILog log)
        {
            _page = page;
            _queueService = urlQueueService;
            _configuration = configuration;
            _pageActionService = pageActionService;
            _outputService = outputService;
            _log = log;

            if (!_configuration.Silent)
            {
                _queueService.Progress += (sender, args) =>
                {
                    _log.Warn($"{args.Remaining} pages remaining. {args.Count} parsed.");
                };
            }
        }

        public void Start()
        {
            if (_configuration.Inclusions.Any())
                _configuration.Inclusions.Iter(i => _queueService.Add(i));

            string url;
            while (_queueService.TryDequeue(out url))
            {
                var response = new Response { Url = url };
                var timer = System.Diagnostics.Stopwatch.StartNew();

                // Load
                _page.NavigateToViewWithJsErrorProxy(response.Url);
                response.TimeTakenMs = timer.ElapsedMilliseconds;

                // Page errors?
                response.JavascriptErrors = _page.GetJavaScriptErrors(TimeSpan.FromMinutes(1));
                response.Code = _page.ReponseCode;

                // Log
                _outputService.Write(response);

                // Selenium scripts for this URL
                timer = System.Diagnostics.Stopwatch.StartNew();
                _pageActionService.Invoke(response.Url);
                _log.Debug($"Page action invoke took {timer.ElapsedMilliseconds} ms");

                // Get links
                if (_configuration.ShouldFollowPageLinks)
                {
                    timer = System.Diagnostics.Stopwatch.StartNew();
                    var links = _page.Links();
                    _log.Debug($"Fetch links took {timer.ElapsedMilliseconds} ms");

                    // Process links
                    timer = System.Diagnostics.Stopwatch.StartNew();
                    links.Iter(l => _queueService.Add(l));
                    _log.Debug($"Process links took {timer.ElapsedMilliseconds} ms");
                }
            }
        }
    }
}
