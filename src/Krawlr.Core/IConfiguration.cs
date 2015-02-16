namespace Krawlr.Core
{
    using System.Collections.Generic;

    public interface IConfiguration
    {
        bool HasError { get; }
        string GetUsage();

        string BaseUrl { get; set; }
        string FollowPageLinks { get; set; }
        bool ShouldFollowPageLinks { get; }
        bool Silent { get; set; }
        int MaxPageLinksToFollow { get; set; }
        string PageScriptsPath { get; set; }
        string ExclusionsFilePath { get; set; }
        string InclusionsFilePath { get; set; }
        IEnumerable<string> Exclusions { get; }
        IEnumerable<string> Inclusions { get; }
        string OutputPath { get; set; }

        // Configuration settings for WebDriver
        string RemoteDriverPath { get; set; }
        string WebDriver { get; set; } // Chrome, Firefox...
        bool WebDriverUseFiddlerProxy { get; set; }
        int WebDriverFiddlerProxyPort { get; set; }
    }
}
