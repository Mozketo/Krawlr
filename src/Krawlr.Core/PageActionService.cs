using System;
using System.Collections.Generic;
using System.Linq;
using Krawlr.Core.Extensions;
using System.IO;
using Jint;
using OpenQA.Selenium;

namespace Krawlr.Core
{
    public interface IPageActionService
    {
        void GenerateInstances(string url);
    }

    public class PageActionService : IPageActionService
    {
        IWebDriver _driver;
        IConfiguration _configuration;

        public PageActionService(IWebDriver driver, IConfiguration configuration)
        {
            _driver = driver;
            _configuration = configuration;
        }

        static Func<string, IEnumerable<string>> readFiles = new Func<string, IEnumerable<string>>(path =>
        {
            if (!Directory.Exists(path))
                return Enumerable.Empty<string>();

            var result = new DirectoryInfo(path).EnumerateFiles("PageAction-*.js").Select(f =>
            {
                return File.ReadAllText(f.FullName);
            });
            return result;
        })
       .Memoize(threadSafe: true);

        public void GenerateInstances(string url)
        {
            var engine = new Engine(cfg => cfg.AllowClr(typeof(By).Assembly))
                .SetValue("url", url)
                .SetValue("driver", _driver);

            var path = _configuration.PageScriptsPath;
            readFiles(path).Iter(source => engine.Execute(source));
        }
    }
}
