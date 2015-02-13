namespace Krawlr.Core.PageActions
{
    public interface IPageAction
    {
        string Url { get; }
        void Invoke(OpenQA.Selenium.IWebDriver driver);
    }
}
