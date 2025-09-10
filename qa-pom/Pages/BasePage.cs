using Core;
using OpenQA.Selenium;

namespace Pages;

public abstract class BasePage
{
    protected readonly IWebDriver Driver;
    protected readonly Waits Wait;

    protected BasePage(IWebDriver driver)
    {
        Driver = driver;
        Wait = new Waits(driver);
    }

    protected IWebElement El(By by) => Wait.UntilClickable(by);
    protected void Type(By by, string text) { var e = El(by); e.Clear(); e.SendKeys(text); }
    protected void Click(By by) => El(by).Click();
    protected string TextOf(By by) => El(by).Text;
}
