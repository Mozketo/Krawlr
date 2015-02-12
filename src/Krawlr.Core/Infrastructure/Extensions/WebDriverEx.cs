using OpenQA.Selenium;

namespace Krawlr.Core.Infrastructure.Extensions
{
    public static class WebDriverExtensions
    {
        public static void Navigate(this IWebDriver driver, string absoluteUrl)
        {
            driver.Navigate().GoToUrl(absoluteUrl);
        }
    }
}
