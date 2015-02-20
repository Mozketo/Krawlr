namespace Krawlr.Core.DTO
{
    using System.Collections.Generic;

    // DTO messages for MQ:
    public class Url { public string Path { get; set; } }

    public class UrlResponse
    {
        public Response Response { get; set; }
        public IEnumerable<string> Links { get; set; }
    }
}
