using Funq;
using Krawlr.Core;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krawlr.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            CommandLine.Parser.Default.ParseArguments(args, options);

            using (var container = new Container())
            {
                string baseUrl = options.Url;

                container.Register<IUrlQueueService>(c => new UrlQueueService(baseUrl)).ReusedWithin(ReuseScope.Container);
                container.Register<IWebDriver>(c => Driver());
                container.Register<Page>(c => new Page(container.Resolve<IWebDriver>()));

                var queueService = container.Resolve<IUrlQueueService>();
                queueService.Add(baseUrl);

                var page = container.Resolve<Page>();
                
                //var links = page.Links()
                //    .Select(el => el.GetAttribute("href")).Distinct()
                //    .Where(t => t.StartsWith(baseUrl, StringComparison.InvariantCultureIgnoreCase));

                //links.ToList().ForEach(l => queueService.Add(l));

                while (queueService.Peek())
                {
                    // Load
                    var url = queueService.Dequeue();
                    page.NavigateToViewWithJsErrorProxy(url);

                    // Log
                    // TODO

                    // Links?
                    var links = page.Links()
                        .Select(el => el.GetAttribute("href")).Distinct()
                        .Where(t => t.StartsWith(baseUrl, StringComparison.InvariantCultureIgnoreCase));
                    links.ToList().ForEach(l => queueService.Add(l));
                }
            }
        }

        static IWebDriver Driver() //this WebDriverSettings settings)
        {
            OpenQA.Selenium.Proxy proxy = null;
            //if (settings.UseFiddlerProxy)
            //{
            //    // Note that we're using a port of 0, which tells Fiddler to
            //    // select a random available port to listen on.
            //    int proxyPort = StartFiddlerProxy(settings.FiddlerProxyPort);

            //    // We are only proxying HTTP traffic, but could just as easily
            //    // proxy HTTPS or FTP traffic.
            //    proxy = new OpenQA.Selenium.Proxy { HttpProxy = String.Format("127.0.0.1:{0}", proxyPort) };
            //}

            //if (settings.BrowserType.Equals("firefox", StringComparison.InvariantCultureIgnoreCase))
            //{
            //    return new OpenQA.Selenium.Firefox.FirefoxDriver();
            //}
            //else if (settings.BrowserType.ToLower().Contains("remote"))
            //{
                var capability = DesiredCapabilities.Chrome();
                if (proxy != null)
                    capability.SetCapability(CapabilityType.Proxy, proxy);
                return new RemoteWebDriver(new Uri("http://localhost:9515"), capability);
            //}
            //return driver;
        }

        //private static int StartFiddlerProxy(int desiredPort)
        //{
        //    // We explicitly do *NOT* want to register this running Fiddler
        //    // instance as the system proxy. This lets us keep isolation.
        //    Console.WriteLine("Starting Fiddler proxy");
        //    FiddlerCoreStartupFlags flags = FiddlerCoreStartupFlags.Default & ~FiddlerCoreStartupFlags.RegisterAsSystemProxy;
        //    FiddlerApplication.Startup(desiredPort, flags);
        //    int proxyPort = FiddlerApplication.oProxy.ListenPort;
        //    Console.WriteLine("Fiddler proxy listening on port {0}", proxyPort);
        //    return proxyPort;
        //}
    }
}
