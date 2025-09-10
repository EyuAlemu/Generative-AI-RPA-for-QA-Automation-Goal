using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Models;

public class TestSuite
{
    public string test_suite_id { get; set; } = "";
    public string source_story_id { get; set; } = "";
    public string app_under_test { get; set; } = "";
    public List<TestCase> test_cases { get; set; } = new();
}

public class TestCase
{
    public string id { get; set; } = "";
    public string title { get; set; } = "";
    public string priority { get; set; } = "P2";
    public List<string> tags { get; set; } = new();
    public string gherkin { get; set; } = "";
    public List<Step> steps { get; set; } = new();
    public List<string> expected_results { get; set; } = new();
}

public class Step
{
    public string action { get; set; } = "";
    public string target { get; set; } = "";
    public string? value { get; set; }
}

public static class SuiteLoader
{
    public static TestSuite LoadFrom(string path) =>
        Newtonsoft.Json.JsonConvert.DeserializeObject<TestSuite>(File.ReadAllText(path))!;
}
