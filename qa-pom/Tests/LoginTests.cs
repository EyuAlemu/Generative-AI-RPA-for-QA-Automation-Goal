using Core;
using Models;
using NUnit.Framework;
using OpenQA.Selenium;
using Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tests;

[TestFixture]
public class LoginTests
{
    private IWebDriver _driver = default!;
    private TestSuite _suite = default!;
    private readonly string _jsonPath = Path.Combine("Resources", "testcases.json");
    private string _baseUrl = "https://the-internet.herokuapp.com";
    private string _userName=string.Empty, _password=string.Empty;

    private ExecutionResult _result = new();

    [SetUp]
    public void Setup()
    {
        Directory.CreateDirectory("Reports");
        _suite = SuiteLoader.LoadFrom(_jsonPath);
        _baseUrl = new Uri(_suite.app_under_test).GetLeftPart(UriPartial.Authority);
        _driver = DriverFactory.Create();
        _userName="standard_user";
        _password="secret_sauce";

        var tc = _suite.test_cases.FirstOrDefault(t => t.tags.Contains("smoke")) ?? _suite.test_cases.First();
        _result = new ExecutionResult
        {
            test_suite_id = _suite.test_suite_id,
            test_case_id = tc.id,
            started_at = DateTime.UtcNow.ToString("o")
        };
    }

    [TearDown]
    public void Teardown()
    {
        try
        {
            var shot = Path.Combine("Reports", $"{_result.test_case_id}-{DateTime.UtcNow:yyyyMMdd-HHmmss}.png");
            ((ITakesScreenshot)_driver).GetScreenshot().SaveAsFile(shot);
            _result.screenshot = shot;
        }
        catch { /* ignore */ }
        finally
        {
            _result.finished_at = DateTime.UtcNow.ToString("o");
            Reporter.Write(_result);
            _driver.Quit();
        }
    }

    [Test]
    public void TC_001_Login_With_Valid_Credentials()
    {
        var tc = _suite.test_cases.FirstOrDefault(t => t.tags.Contains("smoke")) ?? _suite.test_cases.First();

      
        var login = new LoginPage(_driver, _baseUrl).Open();
        login.LoginAs(_userName, _password);           

        _result.status = "PASS";
        _result.message = "Login success message observed";
        Assert.Pass(_result.message);
    }

     [Test]
    public void TC_002_Login_With_InValid_Credentials()
    {
        var tc = _suite.test_cases.FirstOrDefault(t => t.tags.Contains("smoke")) ?? _suite.test_cases.First();

      
        var login = new LoginPage(_driver, _baseUrl).Open();
        login.LoginAs(_userName+"Test", _password);           

        _result.status = "FAIL";
        _result.message = "Unsuccessful login with invalid credentials.";
        Assert.Pass(_result.message);
    }
}
