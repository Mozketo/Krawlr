namespace Krawlr.Core
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using MZMemoize.Extensions;
    using MZMemoize;

    public class Response
    {
        static Func<string, string> DomainPart = new Func<string, string>(url =>
        {
            var uri = new Uri(url);
            return uri.GetLeftPart(UriPartial.Authority);
        })
        .Memoize(threadSafe: true);

        public Response()
        {
            Created = DateTime.Now; // Use local time as opposed to UTC.
        }

        public DateTime Created { get; protected set; }
        public string Url { get; set; }
        public string Domain { get { return DomainPart(this.Url); } }
        public string RelativeUrl { get { return new Uri(Url).PathAndQuery; } }
        public int Code { get; set; }
        public IEnumerable<string> JavascriptErrors { get; set; }
        public bool HasJavscriptErrors { get { return JavascriptErrors.EmptyIfNull().Any(); } }
        public decimal TimeTakenMs { get; set; }
        public string LoggableJavascriptErrors
        {
            get
            {
                return String.Join("\n", JavascriptErrors.EmptyIfNull());
            }
        }

        public override string ToString()
        {
            return $"{Url} took {TimeTakenMs} ms with status code of {Code}. JS Errors? {HasJavscriptErrors}";
        }
    }
}
