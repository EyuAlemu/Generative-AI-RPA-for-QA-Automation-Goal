using Newtonsoft.Json;
using System;
using System.IO;

namespace Core;

public class ExecutionResult
{
    public string test_suite_id { get; set; } = "";
    public string test_case_id  { get; set; } = "";
    public string status        { get; set; } = "FAIL";
    public string message       { get; set; } = "";
    public string screenshot    { get; set; } = "";
    public string started_at    { get; set; } = "";
    public string finished_at   { get; set; } = "";
}

public static class Reporter
{
    public static void Write(ExecutionResult r)
    {
        Directory.CreateDirectory("Reports");
        var path = Path.Combine("Reports", "execution_report.json");
        File.WriteAllText(path, JsonConvert.SerializeObject(r, Formatting.Indented));
        Console.WriteLine($"Report: {path}");
    }
}
