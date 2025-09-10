using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace Core;

public static class DriverFactory
{
    public static IWebDriver Create()
    {
        var opts = new ChromeOptions();
        if (Environment.GetEnvironmentVariable("HEADLESS") == "1")
            opts.AddArgument("--headless=new");
        opts.AddArgument("--window-size=1280,900");
        return new ChromeDriver(opts);
    }
}
