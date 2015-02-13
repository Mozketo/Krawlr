using CommandLine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Krawlr.Core;
using Krawlr.Core.Extensions;
using System.Linq;

namespace Krawlr.Core
{
    public class ConsoleConfiguration : IConfiguration
    {
        public ConsoleConfiguration(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments(args, this);
            FollowPageLinks = true;
        }

        [Option('u', "url", Required = true, HelpText = "URL to start crawling.")]
        public string BaseUrl { get; set; }

        [Option('f', "follow", Required = false, HelpText = "If true after the page is ready it will be checked for all a href links and be added to the queue of pages to load. Default: true")]
        public bool FollowPageLinks { get; set; }

        [Option("exclusions", Required = false, HelpText = "List of URLs that should be ignored.")]
        public string ExclusionsFilePath { get; set; }

        [Option("inclusions", Required = false, HelpText = "List of URLs to follow. When supplied Krawlr will not crawl for links.")]
        public string InclusionsFilePath { get; set; }

        public IEnumerable<string> Exclusions { get { return readFile(ExclusionsFilePath); } }
        public IEnumerable<string> Inclusions { get { return readFile(InclusionsFilePath); } }

        [HelpOption]
        public string GetUsage()
        {
            // this without using CommandLine.Text
            //  or using HelpText.AutoBuild
            var usage = new StringBuilder();
            usage.AppendLine(String.Format("Krawlr {0}.{1}", "0", "0"));
            usage.AppendLine("-u --url http://site.com/");
            usage.AppendLine("--exclude exclude.txt");
            usage.AppendLine("--include include.txt");
            return usage.ToString();
        }

        static Func<string, IEnumerable<string>> readFile = new Func<string, IEnumerable<string>>(path =>
        {
            var result = path.HasValue() && File.Exists(path)
                ? File.ReadAllLines(path)
                : Enumerable.Empty<string>();
            return result;
        })
        .Memoize(threadSafe: true);
    }
}
