using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace Core;

public class Waits
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;

    public Waits(IWebDriver driver, int seconds = 10)
    {
        _driver = driver;
        _wait = new WebDriverWait(new SystemClock(), driver, TimeSpan.FromSeconds(seconds), TimeSpan.FromMilliseconds(250));
    }

    public IWebElement UntilVisible(By by) => _wait.Until(ExpectedConditions.ElementIsVisible(by));
    public IWebElement UntilClickable(By by) => _wait.Until(ExpectedConditions.ElementToBeClickable(by));
    public bool UrlContains(string part) => _wait.Until(d => d.Url.Contains(part, StringComparison.OrdinalIgnoreCase));
}
