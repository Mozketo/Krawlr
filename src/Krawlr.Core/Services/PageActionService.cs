using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Jint;
using OpenQA.Selenium;
using MZMemoize;
using MZMemoize.Extensions;

namespace Krawlr.Core.Services
{
    public interface IPageActionService
    {
        void Invoke(string url);
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

            var result = new DirectoryInfo(path).EnumerateFiles("*.js").Select(f =>
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"Reading Page Action {f.FullName}");
                Console.ResetColor();
                return File.ReadAllText(f.FullName);
            });
            return result.ToList();
        })
       .Memoize(threadSafe: true);

        public void Invoke(string url)
        {
            var engine = new Engine(cfg => cfg.AllowClr(typeof(By).Assembly))
                .SetValue("url", url)
                .SetValue("driver", _driver);

            var path = _configuration.PageScriptsPath;
            readFiles(path).Iter(source => engine.Execute(source));
        }
    }
}
