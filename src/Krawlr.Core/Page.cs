using OpenQA.Selenium;
using System.Collections.Generic;
using Krawlr.Core.Extensions;
using Fiddler;
using System.Text.RegularExpressions;
using System;
using System.Linq;
using HtmlAgilityPack;
using MZMemoize.Extensions;

namespace Krawlr.Core
{
    public class Page
    {
        protected ILog _log;

        public IWebDriver Driver { get; protected set; }
        public virtual int ReponseCode { get; protected set; }

        public Page(IWebDriver driver, ILog log)
        {
            Driver = driver;
            _log = log;
        }

        public IEnumerable<string> Links()
        {
            //var doc = new HtmlDocument();
            //doc.LoadHtml(Driver.PageSource);

            //var links = doc.DocumentNode.SelectNodes("//a")
            //    .EmptyIfNull()
            //    .Select(p => p.GetAttributeValue("href", String.Empty))
            //    .Distinct();
            //return links;

            var links = Driver.FindElements(By.TagName("a")).Select(el =>
                {
                    try { return el.GetAttribute("href"); }
                    catch { return null; }
                });
            return links;
        }

        public void NavigateToViewWithJsErrorProxy(string targetUrl)
        {
            string errorScript =
                    @"window.__webdriver_javascript_errors = [];
            window.onerror = function(errorMsg, url, line) {
            window.__webdriver_javascript_errors.push(
                errorMsg + ' (found at ' + url + ', line ' + line + ')');
            };";

            SessionStateHandler responseheadersAvailable = delegate (Session targetSession)
            {
                // Tell Fiddler to buffer the response so that we can modify
                // it before it gets back to the browser.
                ReponseCode = targetSession.responseCode;
            };

            SessionStateHandler beforeRequestHandler = delegate (Session targetSession)
            {
                // Tell Fiddler to buffer the response so that we can modify
                // it before it gets back to the browser.
                targetSession.bBufferResponse = true;
            };

            SessionStateHandler beforeResponseHandler = delegate (Session targetSession)
            {
                if (targetSession.fullUrl.ContainsEx(targetUrl) &&
                    targetSession.oResponse.headers.ExistsAndContains("Content-Type", "html"))
                {
                    targetSession.utilDecodeResponse();
                    string responseBody = targetSession.GetResponseBodyAsString();
                    string headTag = Regex.Match(responseBody, "<head.*>", RegexOptions.IgnoreCase).ToString();
                    string addition = headTag + "<script>" + errorScript + "</script>";
                    targetSession.utilReplaceOnceInResponse(headTag, addition, false);
                }
            };

            FiddlerApplication.BeforeRequest += beforeRequestHandler;
            FiddlerApplication.BeforeResponse += beforeResponseHandler;
            FiddlerApplication.ResponseHeadersAvailable += responseheadersAvailable;
            Driver.Navigate(targetUrl);
            FiddlerApplication.BeforeResponse -= beforeResponseHandler;
            FiddlerApplication.BeforeRequest -= beforeRequestHandler;
            FiddlerApplication.ResponseHeadersAvailable -= responseheadersAvailable;
        }

        public IEnumerable<string> GetJavaScriptErrors(TimeSpan timeout)
        {
            string errorRetrievalScript =
                    @"var errorList = window.__webdriver_javascript_errors;
            window.__webdriver_javascript_errors = [];
            return errorList;";

            var endTime = DateTime.Now.Add(timeout);
            var errorList = new List<string>();
            var executor = Driver as IJavaScriptExecutor;
            IEnumerable<object> result;

            try
            {
                result = executor.ExecuteScript(errorRetrievalScript) as IEnumerable<object>;
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
                return null;
            }

            while (result == null && DateTime.Now < endTime)
            {
                try
                {
                    result = executor.ExecuteScript(errorRetrievalScript) as IEnumerable<object>;
                }
                catch (Exception ex)
                {
                    _log.Error(ex.ToString());
                }
            }

            if (result == null)
                return null;

            return result.Select(m => m.ToString());
        }
    }
}
