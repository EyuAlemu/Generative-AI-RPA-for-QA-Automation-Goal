using OpenQA.Selenium;

namespace Pages;

public class LoginPage : BasePage
{
    private static readonly By Username = By.CssSelector("#user-name");
    private static readonly By Password = By.CssSelector("#password");
    private static readonly By Submit   = By.CssSelector("input[type='submit']");
    private static readonly By Flash    = By.CssSelector("#flash");

    private readonly string _url;

    public LoginPage(IWebDriver driver, string baseUrl) : base(driver)
    {
        _url = baseUrl.TrimEnd('/');
    }

    public LoginPage Open()
    {
        //Driver.Navigate().GoToUrl($"{_url}/login");
        Driver.Navigate().GoToUrl($"{_url}");
        Driver.Manage().Window.Maximize();
        
        
        return this;
    }

    public LoginPage LoginAs(string user, string pass)
    {
        Type(Username, user);
        Type(Password, pass);
        Click(Submit);
        return this;
    }

    public string FlashText() => TextOf(Flash);
}
