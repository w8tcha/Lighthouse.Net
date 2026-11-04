using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using AwesomeAssertions;

using LighthousePlaywright.Net.Core;
using LighthousePlaywright.Net.Objects;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

namespace LighthousePlaywright.Net.Tests.Unit;

public class ScriptMakerTests
{
    private static readonly string NodeModulesPath = OperatingSystem.IsWindows()
        ? @"C:\Users\me\AppData\Roaming\npm\node_modules"
        : "/usr/lib/node_modules";

    private static JObject ExtractOptions(string script)
    {
        var match = Regex.Match(script, @"const opts = (\{.*\});");
        match.Success.Should().BeTrue("the script should declare the options object");
        return JObject.Parse(match.Groups[1].Value);
    }

    private static JObject ExtractObject(string script, string key)
    {
        var match = Regex.Match(script, key + @": (\{.*\}),?\r?$", RegexOptions.Multiline);
        match.Success.Should().BeTrue($"the script should contain '{key}'");
        return JObject.Parse(match.Groups[1].Value);
    }

    [Test]
    public void Produce_NullRequest_ReturnsNull()
    {
        new ScriptMaker(new Options()).Produce(null, NodeModulesPath).Should().BeNull();
    }

    [Test]
    public void Produce_ReplacesAllPlaceholders()
    {
        var script = new ScriptMaker(new Options()).Produce(new AuditRequest("http://example.com"), NodeModulesPath);

        script.Should().NotMatchRegex(@"\{(OPTIONS|REPORTS|THRESHOLDS|URL|PORT|PLAYWRIGHT_LIGHTHOUSE|PLAYWRIGHT)\}");
    }

    [Test]
    public void Produce_InsertsUrlAsJsonString()
    {
        var script = new ScriptMaker(new Options())
            .Produce(new AuditRequest("https://example.com/?q=\"quoted\"'single'"), NodeModulesPath);

        script.Should().Contain("""launchChromeAndRunLighthouse("https://example.com/?q=\"quoted\"'single'", opts)""");
    }

    [Test]
    public void Produce_InsertsPort()
    {
        var options = new Options { Port = 9333 };

        var script = new ScriptMaker(options).Produce(new AuditRequest("http://example.com"), NodeModulesPath);

        script.Should().Contain("'--remote-debugging-port=9333'");
        script.Should().Contain("port: 9333");
    }

    [Test]
    [Platform("Win")]
    public void Produce_WindowsNodeModulesPath_IsEscaped()
    {
        var script = new ScriptMaker(new Options()).Produce(new AuditRequest("http://example.com"), NodeModulesPath);

        script.Should().Contain(@"require(""C:\\Users\\me\\AppData\\Roaming\\npm\\node_modules\\playwright-lighthouse"")");
        script.Should().Contain(@"require(""C:\\Users\\me\\AppData\\Roaming\\npm\\node_modules\\playwright"")");
    }

    [Test]
    [Platform("Linux")]
    public void Produce_LinuxNodeModulesPath_UsesForwardSlashes()
    {
        var script = new ScriptMaker(new Options()).Produce(new AuditRequest("http://example.com"), NodeModulesPath);

        script.Should().Contain("""require("/usr/lib/node_modules/playwright-lighthouse")""");
        script.Should().Contain("""require("/usr/lib/node_modules/playwright")""");
    }

    [Test]
    public void Produce_NodeModulesPathWithQuote_StaysValidJsString()
    {
        var path = Path.Combine(Path.GetTempPath(), "o'brien");

        var script = new ScriptMaker(new Options()).Produce(new AuditRequest("http://example.com"), path);

        var match = Regex.Match(script, @"require\((""(?:[^""\\]|\\.)*"")\)");
        match.Success.Should().BeTrue();
        JToken.Parse(match.Groups[1].Value).Value<string>()
            .Should().Be(Path.Combine(path, "playwright-lighthouse"));
    }

    [Test]
    public void Produce_DefaultRequest_OnlyEmitsChromeFlags()
    {
        var script = new ScriptMaker(new Options()).Produce(new AuditRequest("http://example.com"), NodeModulesPath);

        var opts = ExtractOptions(script);

        opts.Properties().Select(p => p.Name).Should().Equal("chromeFlags");
        opts["chromeFlags"]!.ToObject<string[]>().Should()
            .Equal("--show-paint-rects", "--headless", "--no-sandbox");
    }

    [Test]
    public void Produce_MapsAllRequestOptions()
    {
        var request = new AuditRequest("http://example.com")
        {
            MaxWaitForLoad = 30000,
            BlockedUrlPatterns = ["*.png", "*ads*"],
            DisableStorageReset = true,
            DisableDeviceEmulation = false,
            OnlyCategories = [Category.Performance, Category.SEO],
            EmulatedFormFactor = AuditRequest.FormFactor.Desktop
        };

        var opts = ExtractOptions(new ScriptMaker(new Options()).Produce(request, NodeModulesPath));

        opts["maxWaitForLoad"]!.Value<int>().Should().Be(30000);
        opts["blockedUrlPatterns"]!.ToObject<string[]>().Should().Equal("*.png", "*ads*");
        opts["disableStorageReset"]!.Value<bool>().Should().BeTrue();
        opts["disableDeviceEmulation"]!.Value<bool>().Should().BeFalse();
        opts["onlyCategories"]!.ToObject<string[]>().Should().Equal("performance", "seo");
        opts["preset"]!.Value<string>().Should().Be("desktop");
    }

    [TestCase(AuditRequest.FormFactor.Mobile, "mobile")]
    [TestCase(AuditRequest.FormFactor.Desktop, "desktop")]
    [TestCase(AuditRequest.FormFactor.None, "none")]
    public void Produce_MapsFormFactorToLowercasePreset(AuditRequest.FormFactor formFactor, string expected)
    {
        var request = new AuditRequest("http://example.com") { EmulatedFormFactor = formFactor };

        var opts = ExtractOptions(new ScriptMaker(new Options()).Produce(request, NodeModulesPath));

        opts["preset"]!.Value<string>().Should().Be(expected);
    }

    [Test]
    public void Produce_SerializesThresholdsWithoutNullPwa()
    {
        var options = new Options { Thresholds = new Thresholds { Performance = 90 } };

        var thresholds = ExtractObject(new ScriptMaker(options).Produce(new AuditRequest("http://example.com"), NodeModulesPath),
            "thresholds");

        thresholds["performance"]!.Value<int>().Should().Be(90);
        thresholds["accessibility"]!.Value<int>().Should().Be(50);
        thresholds.ContainsKey("pwa").Should().BeFalse();
    }

    [Test]
    public void Produce_SerializesReports()
    {
        var options = new Options
        {
            Reports = new Reports { Name = "report", Directory = "dir", Formats = new Formats { Html = true } }
        };

        var reports = ExtractObject(new ScriptMaker(options).Produce(new AuditRequest("http://example.com"), NodeModulesPath),
            "reports");

        reports["name"]!.Value<string>().Should().Be("report");
        reports["directory"]!.Value<string>().Should().Be("dir");
        reports["formats"]!["html"]!.Value<bool>().Should().BeTrue();
    }

    [Test]
    public void Save_WritesContentToUniqueTempFile()
    {
        var first = new ScriptMaker(new Options());
        var second = new ScriptMaker(new Options());

        try
        {
            first.Save("console.log(1);").Should().BeTrue();
            second.Save("console.log(2);").Should().BeTrue();

            first.TempFileName.Should().EndWith(".js");
            Path.GetFileName(first.TempFileName).Should().StartWith("lighthouse-net-");
            first.TempFileName.Should().NotBe(second.TempFileName);
            File.ReadAllText(first.TempFileName).Should().Be("console.log(1);");
        }
        finally
        {
            first.Delete();
            second.Delete();
        }
    }

    [Test]
    public void Delete_RemovesFileAndTempDirectory()
    {
        var sm = new ScriptMaker(new Options());
        sm.Save("x").Should().BeTrue();
        var file = sm.TempFileName;
        var directory = Path.GetDirectoryName(file);

        sm.Delete().Should().BeTrue();

        File.Exists(file).Should().BeFalse();
        Directory.Exists(directory).Should().BeFalse();
    }

    [Test]
    public void Delete_WithoutSave_ReturnsFalse()
    {
        new ScriptMaker(new Options()).Delete().Should().BeFalse();
    }

    [Test]
    public void Save_NullContent_WritesEmptyFile()
    {
        var sm = new ScriptMaker(new Options());
        try
        {
            sm.Save(null).Should().BeTrue();
            File.ReadAllText(sm.TempFileName).Should().BeEmpty();
        }
        finally
        {
            sm.Delete();
        }
    }
}
