using DryIoc;
using Krawlr.Core;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Krawlr.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var container = new Container())
            {
                container.RegisterDelegate<IConfiguration>(r => new ConsoleConfiguration(args), Reuse.Singleton);
                var options = container.Resolve<IConfiguration>();
                if (options.HasError)
                    return;

                container.RegisterDelegate<IUrlQueueService>(r => 
                    new UrlQueueService(r.Resolve<IConfiguration>()), Reuse.Singleton);

                container.Register<IWebDriverService, WebDriverService>();
                container.RegisterDelegate<IWebDriver>(r => r.Resolve<IWebDriverService>().Get(), Reuse.Singleton);

                container.Register<IPageActionService, PageActionService>();
                container.Register<Page, Page>(); // (r => new Page(r.Resolve<IWebDriver>(), baseUrl));
                container.RegisterDelegate<Application>(r => 
                    new Application(
                        r.Resolve<Page>(), 
                        r.Resolve<IUrlQueueService>(),
                        r.Resolve<IConfiguration>(),
                        r.Resolve<IPageActionService>()), 
                    Reuse.Singleton);

                var queueService = container.Resolve<IUrlQueueService>();
                queueService.Add(options.BaseUrl);

                Task.Run(() => container.Resolve<Application>().Start());

                System.Console.WriteLine("Press any key to halt processing...");
                System.Console.ReadKey();
            }
        }
    }
}
