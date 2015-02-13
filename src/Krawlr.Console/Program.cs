using DryIoc;
using Krawlr.Core;
using Krawlr.Core.PageActions;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Krawlr.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            //var options = new Options();
            //CommandLine.Parser.Default.ParseArguments(args, options);

            using (var container = new Container())
            {
                container.RegisterDelegate<IConfiguration>(r => new ConsoleConfiguration(args), Reuse.Singleton);
                var options = container.Resolve<IConfiguration>();

                container.Register<IPageAction, LoginAction>();

                container.RegisterDelegate<IUrlQueueService>(r => 
                    new UrlQueueService(r.Resolve<IConfiguration>()), Reuse.Singleton);
                container.RegisterDelegate<IWebDriver>(r => Driver());
                container.Register<Page, Page>(); // (r => new Page(r.Resolve<IWebDriver>(), baseUrl));
                container.RegisterDelegate<Application>(r => 
                    new Application(
                        r.Resolve<Page>(), 
                        r.Resolve<IUrlQueueService>(),
                        r.Resolve<IEnumerable<IPageAction>>(), 
                        r.Resolve<IConfiguration>()), 
                    Reuse.Singleton);

                var queueService = container.Resolve<IUrlQueueService>();
                queueService.Add(options.BaseUrl);

                Task.Run(() => container.Resolve<Application>().Start());

                System.Console.WriteLine("Press any key to halt processing...");
                System.Console.ReadKey();
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
