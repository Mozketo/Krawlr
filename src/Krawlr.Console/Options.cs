using CommandLine;
using System;
using System.Text;

namespace Krawlr.Console
{
    class Options
    {
        [Option('u', "url", Required = true, HelpText = "URL to start crawling.")]
        public string Url { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            // this without using CommandLine.Text
            //  or using HelpText.AutoBuild
            var usage = new StringBuilder();
            usage.AppendLine(String.Format("Krawlr {0}.{1}", "0", "0"));
            usage.AppendLine("-u -url http://site.com/");
            return usage.ToString();
        }
    }
}
