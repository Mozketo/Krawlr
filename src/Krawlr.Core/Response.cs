namespace Krawlr.Core
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

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
        public bool HasJavscriptErrors { get { return JavascriptErrors.Any(); } }
        public decimal TimeTakenMs { get; set; }
        public string LoggableJavascriptErrors
        {
            get
            {
                return String.Join("\n", JavascriptErrors);
            }
        }

        public override string ToString()
        {
            return $"Navigating in {TimeTakenMs} ms to {Url} with status code of {Code}. JS Errors? {HasJavscriptErrors}";
        }
    }
}
