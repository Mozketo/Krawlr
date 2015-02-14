// *********************************
//
// Sample Page Action to execute
// webdriver DOM manipulation.
//
// Copyright Ben Clark-Robinson 2015
//
//**********************************
//
// Need to run Web Driver (Selenium) actions against a page? Using a PageAction allows for DOM
// manipulation. I'm ensuring that this script is only executed for a given URL.
// Note: each Page Action needs to be named like "PageAction-*.js" to be automatically read
// By Krawlr

if (url.indexOf("auth/login") > -1)
{
    // Get an instance of the Webdriver namespace. We'll use this to access the By keyword.
    var selenium = importNamespace('OpenQA.Selenium');
    // driver (aka Web Driver) is injected into each Page Action.
    // Let's now manipulate the DOM.
    var element = driver.FindElement(selenium.By.Id("Username"));
    element.Clear();
    element.SendKeys("");

    element = driver.FindElement(selenium.By.Id("Password"));
    element.Clear();
    element.SendKeys("");

    // Finally find the submit button on the login form and action it.
    driver.FindElement(selenium.By.CssSelector("footer button")).Submit();
}