using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

using AwesomeAssertions;

using LighthousePlaywright.Net.Core;
using LighthousePlaywright.Net.Objects;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using NUnit.Framework;

namespace LighthousePlaywright.Net.Tests.Unit;

public class OptionsTests
{
    [Test]
    public void Defaults_AreInitialized()
    {
        var options = new Options();

        options.Reports.Should().NotBeNull();
        options.Thresholds.Should().NotBeNull();
        options.Port.Should().BeInRange(1, 65535);
    }

    [Test]
    public void Port_IsFreeLoopbackPort()
    {
        var options = new Options();

        // The port must be bindable right after being chosen.
        using var listener = new TcpListener(IPAddress.Loopback, options.Port);
        var act = () => listener.Start();

        act.Should().NotThrow();
        listener.Stop();
    }

    [Test]
    public void Thresholds_Defaults()
    {
        var thresholds = new Thresholds();

        thresholds.Performance.Should().Be(50);
        thresholds.Accessibility.Should().Be(50);
        thresholds.BestPractices.Should().Be(50);
        thresholds.Seo.Should().Be(50);
        thresholds.Pwa.Should().BeNull();
    }

    [Test]
    public void Thresholds_SerializeWithLighthouseNames()
    {
        var json = JObject.Parse(JsonConvert.SerializeObject(new Thresholds { Pwa = 10 }));

        json.Properties().Select(p => p.Name).Should()
            .BeEquivalentTo("performance", "accessibility", "best-practices", "seo", "pwa");
        json["pwa"]!.Value<int>().Should().Be(10);
    }

    [Test]
    public void Reports_Defaults()
    {
        var before = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var reports = new Reports();
        var after = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        reports.Directory.Should().Be("lighthouse");
        reports.Formats.Html.Should().BeFalse();
        reports.Formats.Json.Should().BeFalse();
        reports.Formats.Csv.Should().BeFalse();
        reports.Name.Should().StartWith("lighthouse-");
        long.Parse(reports.Name["lighthouse-".Length..]).Should().BeInRange(before, after);
    }

    [Test]
    public void Reports_SerializeWithPlaywrightLighthouseNames()
    {
        var reports = new Reports
        {
            Name = "my-report",
            Directory = "out",
            Formats = new Formats { Html = true, Json = true, Csv = false }
        };

        var json = JObject.Parse(JsonConvert.SerializeObject(reports));

        json["name"]!.Value<string>().Should().Be("my-report");
        json["directory"]!.Value<string>().Should().Be("out");
        json["formats"]!["html"]!.Value<bool>().Should().BeTrue();
        json["formats"]!["json"]!.Value<bool>().Should().BeTrue();
        json["formats"]!["csv"]!.Value<bool>().Should().BeFalse();
    }

    [Test]
    public void Screenshot_ExposesData()
    {
        new Screenshot("abc").Base64Data.Should().Be("abc");
        default(Screenshot).Base64Data.Should().BeNull();
    }

    [Test]
    public void OSystem_IsLinux_MatchesRuntime()
    {
        OSystem.IsLinux.Should().Be(!OperatingSystem.IsWindows());
    }
}
