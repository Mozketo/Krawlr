using Fiddler;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Linq;
using Krawlr.Core.Extensions;

namespace Krawlr.Core
{
    public interface IWebDriverService
    {
        IWebDriver Get();
    }

    public class WebDriverService : IWebDriverService
    {
        protected IConfiguration _configuration;

        public WebDriverService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IWebDriver Get()
        {
            OpenQA.Selenium.Proxy proxy = null;

            if (_configuration.WebDriverUseFiddlerProxy)
            {
                // Note that we're using a port of 0, which tells Fiddler to
                // select a random available port to listen on.
                int proxyPort = StartFiddlerProxy(_configuration.WebDriverFiddlerProxyPort);

                // We are only proxying HTTP traffic, but could just as easily
                // proxy HTTPS or FTP traffic.
                proxy = new OpenQA.Selenium.Proxy { HttpProxy = String.Format("127.0.0.1:{0}", proxyPort) };
            }

            if (_configuration.WebDriver.EqualsEx("firefox"))
            {
                return new OpenQA.Selenium.Firefox.FirefoxDriver();
            }
            
            var capability = DesiredCapabilities.Chrome();
            if (proxy != null)
                capability.SetCapability(CapabilityType.Proxy, proxy);
            return new RemoteWebDriver(new Uri("http://localhost:9515"), capability);
        }

        protected int StartFiddlerProxy(int desiredPort)
        {
            // We explicitly do *NOT* want to register this running Fiddler instance as the system proxy. 
            // This lets us keep isolation.
            FiddlerCoreStartupFlags flags = FiddlerCoreStartupFlags.Default & ~FiddlerCoreStartupFlags.RegisterAsSystemProxy;
            FiddlerApplication.Startup(desiredPort, flags);

            int proxyPort = FiddlerApplication.oProxy.ListenPort;
            return proxyPort;
        }
    }
}
