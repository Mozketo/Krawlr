namespace Krawlr.Core.Services
{
    using Krawlr.Core.DTO;
    using System;
    using System.Linq;
    using ServiceStack.Messaging;    

    public interface IListen
    {
        void Listen();
    }

    public class NavigationListener : IListen
    {
        protected ILog _log;
        protected IConfiguration _configuration;
        protected IMessageService _mQServer;
        protected Page _page;
        protected IPageActionService _actionService;

        public NavigationListener(IConfiguration configuration, IMessageService messageService, 
            IPageActionService actionService, Page page, ILog log)
        {
            _configuration = configuration;
            _mQServer = messageService;
            _log = log;
            _page = page;
            _actionService = actionService;
        }

        public void Listen()
        {
            _mQServer.RegisterHandler<Url>(m =>
            {
                var response = new Response { Url = m.GetBody().Path };
                _log.Debug($"Navigate starting to {response.Url}");
                var timer = System.Diagnostics.Stopwatch.StartNew();

                // Load
                _page.NavigateToViewWithJsErrorProxy(response.Url);
                response.TimeTakenMs = timer.ElapsedMilliseconds;

                // Page errors?
                response.JavascriptErrors = _page.GetJavaScriptErrors(TimeSpan.FromMinutes(1));
                response.Code = _page.ReponseCode;

                // Selenium scripts for this URL
                _actionService.Invoke(response.Url);

                // Get links
                if (_configuration.FollowPageLinks)
                {
                    timer = System.Diagnostics.Stopwatch.StartNew();
                    var links = _page.Links();
                    _log.Debug($"Fetch links took {timer.ElapsedMilliseconds} ms");
                    return new UrlResponse { Response = response, Links = links };
                }

                return Enumerable.Empty<string>();
            });
            _mQServer.Start();
        }
    }
}
