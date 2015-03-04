using Fiddler;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Diagnostics;
using MZMemoize.Extensions;

namespace Krawlr.Core.Services
{
    public interface IWebDriverService
    {
        IWebDriver Get();
    }

    public class WebDriverService : BaseWebDriverService, IWebDriverService
    {
        protected IConfiguration _configuration;
        protected ILog _log;
        protected Process _process;

        public WebDriverService(IConfiguration configuration, ILog log)
        {
            _configuration = configuration;
            _log = log;
        }

        public IWebDriver Get()
        {
            OpenQA.Selenium.Proxy proxy = null;
            if (_configuration.WebDriver.UseFiddlerProxy)
                proxy = base.GetProxy(_configuration);

            var capabilities = _configuration.WebDriver.Driver.EqualsEx("firefox")
                ? DesiredCapabilities.Firefox()
                : DesiredCapabilities.Chrome();

            if (proxy != null)
                capabilities.SetCapability(CapabilityType.Proxy, proxy);

            if (_configuration.WebDriver.Driver.EqualsEx("firefox"))
            {
                return new OpenQA.Selenium.Firefox.FirefoxDriver(capabilities);
            }
            return new RemoteWebDriver(new Uri("http://localhost:9515"), capabilities);
        }
    }

    public class IEWebDriverService : BaseWebDriverService, IWebDriverService
    {
        protected IConfiguration _configuration;
        protected ILog _log;
        protected Process _process;

        public IEWebDriverService(IConfiguration configuration, ILog log)
        {
            _configuration = configuration;
            _log = log;
        }

        public IWebDriver Get()
        {
            OpenQA.Selenium.Proxy proxy = null;
            if (_configuration.WebDriver.UseFiddlerProxy)
                proxy = base.GetProxy(_configuration);

            var options = new OpenQA.Selenium.IE.InternetExplorerOptions();
            options.EnsureCleanSession = true;
            options.Proxy = proxy;

            return new OpenQA.Selenium.IE.InternetExplorerDriver(options);
        }
    }

    public abstract class BaseWebDriverService
    {
        protected virtual OpenQA.Selenium.Proxy GetProxy(IConfiguration configuration)
        {
            // Note that we're using a port of 0, which tells Fiddler to
            // select a random available port to listen on.
            int proxyPort = StartFiddlerProxy(configuration.WebDriver.FiddlerProxyPort);

            // We are only proxying HTTP traffic, but could just as easily
            // proxy HTTPS or FTP traffic.
            return new OpenQA.Selenium.Proxy { HttpProxy = String.Format("127.0.0.1:{0}", proxyPort) };
        }

        protected virtual int StartFiddlerProxy(int desiredPort)
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
