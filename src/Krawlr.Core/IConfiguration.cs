namespace Krawlr.Core
{
    using System.Collections.Generic;

    public enum DistributionMode
    {
        ClientServer = 0,
        Server,
        Client
    }

    public interface IConfiguration
    {
        bool HasError { get; }

        // Extra information stating if running in Client / Server mode, and if so
        // which config settings to use
        DistributionMode DistributionMode { get; }

        string BaseUrl { get; }
        bool FollowPageLinks { get; }
        bool Quiet { get; }
        bool IgnoreGuids { get; }
        int MaxPageLinksToFollow { get; }
        string PageScriptsPath { get; }
        string ExclusionsFilePath { get; }
        string InclusionsFilePath { get; }
        IEnumerable<string> Exclusions { get; }
        IEnumerable<string> Inclusions { get; }
        string OutputPath { get; }

        // Configuration settings for WebDriver
        IConfigurationWebDriver WebDriver { get; set; }
    }

    public interface IConfigurationWebDriver
    {
        string Driver { get; set; } // Chrome, Firefox...
        bool UseFiddlerProxy { get; set; }
        int FiddlerProxyPort { get; set; }
    }
}
