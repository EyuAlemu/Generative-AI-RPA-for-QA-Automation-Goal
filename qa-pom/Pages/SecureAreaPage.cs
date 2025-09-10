using Core;
using OpenQA.Selenium;

namespace Pages;

public class SecureAreaPage : BasePage
{
    private static readonly By Header = By.CssSelector("div#content h2"); // "Secure Area"

    public SecureAreaPage(IWebDriver driver) : base(driver) {}

    public bool IsLoaded()
    {
        var ok = Wait.UrlContains("/secure");
        try { _ = El(Header); } catch { ok = false; }
        return ok;
    }
}
