namespace Krawlr.Core
{
    using System.Collections.Generic;

    public interface IConfiguration
    {
        string BaseUrl { get; set; }
        bool FollowPageLinks { get; set; }
        string PageScriptsPath { get; set; }
        string ExclusionsFilePath { get; set; }
        string InclusionsFilePath { get; set; }
        IEnumerable<string> Exclusions { get; }
        IEnumerable<string> Inclusions { get; }
    }
}
