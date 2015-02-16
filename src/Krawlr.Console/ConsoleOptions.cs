using CommandLine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Krawlr.Core.Extensions;
using System.Linq;
using CommandLine.Text;

namespace Krawlr.Core
{
    public class ConsoleConfiguration : IConfiguration
    {
        public bool HasError { get; protected set; }

        public ConsoleConfiguration(string[] args)
        {
            FollowPageLinks = true;
            PageScriptsPath = Path.GetDirectoryName(typeof(Application).Assembly.Location);
            WebDriver = "Chrome";
            WebDriverUseFiddlerProxy = true;

            HasError = !CommandLine.Parser.Default.ParseArguments(args, this);
        }

        [Option('u', "url", Required = true, HelpText = "URL to start crawling.")]
        public string BaseUrl { get; set; }

        [Option('s', "silent", Required = true, HelpText = "URL to start crawling. (Default: false)")]
        public bool Silent { get; set; }

        [Option('f', "follow-links", Required = false, HelpText = "After loading a page should links on the page be followed? (Default: true)")]
        public bool FollowPageLinks { get; set; }

        [Option("max-follow-links", Required = false, HelpText = "Limit the number of pages to crawl. Default: 0 (no limit)")]
        public int MaxPageLinksToFollow { get; set; }

        [Option('e', "exclusions", Required = false, HelpText = "Path to a file with list of routes/keywords in URL to bypass.")]
        public string ExclusionsFilePath { get; set; }

        [Option('i', "inclusions", Required = false, HelpText = "Path to a file with a hard list of routes to hit (will follow in order). Use with --follow-links false")]
        public string InclusionsFilePath { get; set; }

        [Option('s', "scripts", Required = false, HelpText = "After each page is loaded a script may be executed against the page to manipulate the DOM. Recommended for adding Login support to the crawl.")]
        public string PageScriptsPath { get; set; }

        [Option('o', "output", Required = false, HelpText = "Write crawling activity to CSV file with path...")]
        public string OutputPath { get; set; }

        [Option("remotedriver", Required = false, HelpText = "Path to Remote Driver (aka ChromeDriver.exe). Process will be spawned on application start. (Required: If using Chrome)")]
        public string RemoteDriverPath { get; set; }

        public IEnumerable<string> Exclusions { get { return readFile(ExclusionsFilePath); } }
        public IEnumerable<string> Inclusions { get { return readFile(InclusionsFilePath); } }

        // WebDriver configuration and Fiddler proxy
        [Option('w', "webdriver", Required = false, HelpText = "Define WebDriver to use. Firefox, Chrome, Remote (Default: Chrome)")]
        public string WebDriver { get; set; }
        [Option("webdriver-proxy", Required = false, HelpText = "Using Chrome or Remote should route via Fiddler Core? (Default: true) ")]
        public bool WebDriverUseFiddlerProxy { get; set; }
        [Option("webdriver-proxy-port", Required = false, HelpText = "If WebDriver proxy is engaged define the port to use. (Default: 0 (autoselect))")]
        public int WebDriverFiddlerProxyPort { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = HelpText.AutoBuild(this);
            help.Copyright = "Copyright 2015 Ben Clark-Robinson";
            return help.ToString();
        }

        static Func<string, IEnumerable<string>> readFile = new Func<string, IEnumerable<string>>(path =>
        {
            // Ignore any lines that start with backticks "`". They're treated as comments
            var result = path.ExistsEx()
                ? File.ReadAllLines(path).Where(l => l.StartsWith("`") == false)
                : Enumerable.Empty<string>();
            return result;
        })
        .Memoize(threadSafe: true);
    }
}
