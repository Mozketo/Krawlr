using Fiddler;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Linq;
using Krawlr.Core.Extensions;
using System.Diagnostics;

namespace Krawlr.Core.Services
{
    public interface IWebDriverService
    {
        IWebDriver Get();
        void StartRemoteDriverIf();
    }

    public class WebDriverService : IWebDriverService
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

            if (_configuration.WebDriverUseFiddlerProxy)
            {
                // Note that we're using a port of 0, which tells Fiddler to
                // select a random available port to listen on.
                int proxyPort = StartFiddlerProxy(_configuration.WebDriverFiddlerProxyPort);

                // We are only proxying HTTP traffic, but could just as easily
                // proxy HTTPS or FTP traffic.
                proxy = new OpenQA.Selenium.Proxy { HttpProxy = String.Format("127.0.0.1:{0}", proxyPort) };
            }

            var capabilities = _configuration.WebDriver.EqualsEx("firefox")
                ? DesiredCapabilities.Firefox()
                : DesiredCapabilities.Chrome();
            if (proxy != null)
                capabilities.SetCapability(CapabilityType.Proxy, proxy);

            if (_configuration.WebDriver.EqualsEx("firefox"))
            {
                return new OpenQA.Selenium.Firefox.FirefoxDriver(capabilities);
            }
            return new RemoteWebDriver(new Uri("http://localhost:9515"), capabilities);
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

        public void StartRemoteDriverIf()
        {
            var path = _configuration.RemoteDriverPath;
            if (!path.ExistsEx())
                return;

            _log.Info($"Starting remote driver from {path}");

            _process = Process.Start(new ProcessStartInfo
            {
                FileName = path,
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                UseShellExecute = true,
            });
        }
    }
}
