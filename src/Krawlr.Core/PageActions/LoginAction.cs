namespace Krawlr.Core.PageActions
{
    using OpenQA.Selenium;
    using System;

    [Obsolete]
    public class LoginAction
    {
        public string Url { get { return "auth/loginX"; } }
        public void Invoke(IWebDriver driver)
        {
            Func<By, string, bool> sendKey = (by, keys) =>
            {
                var element = driver.FindElement(by);
                element.Clear();
                element.SendKeys(keys);
                return true;
            };
            
            sendKey(By.Id("Username"), "admin");
            sendKey(By.Id("Password"), "");
            driver.FindElement(By.CssSelector("footer button")).Submit();
        }
    }
}
