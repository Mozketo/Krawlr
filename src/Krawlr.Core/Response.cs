namespace Krawlr.Core
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using MZMemoize.Extensions;

    public class Response
    {
        public Response()
        {
            Created = DateTime.Now; // Use local time as opposed to UTC.
        }

        public DateTime Created { get; protected set; }
        public string Url { get; set; }
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
