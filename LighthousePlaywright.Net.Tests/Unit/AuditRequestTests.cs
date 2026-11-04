using System;

using AwesomeAssertions;

using LighthousePlaywright.Net.Objects;

using NUnit.Framework;

namespace LighthousePlaywright.Net.Tests.Unit;

public class AuditRequestTests
{
    [TestCase("http://example.com")]
    [TestCase("https://example.com/path?q=1")]
    public void Constructor_WithProtocol_SetsUrl(string url)
    {
        var request = new AuditRequest(url);

        request.Url.Should().Be(url);
    }

    [TestCase(null)]
    [TestCase("")]
    public void Constructor_NullOrEmpty_Throws(string url)
    {
        var act = () => new AuditRequest(url);

        act.Should().Throw<ArgumentNullException>().WithParameterName("urlWithProtocol");
    }

    [TestCase("example.com")]
    [TestCase("ftp://example.com")]
    public void Constructor_WithoutHttpProtocol_Throws(string url)
    {
        var act = () => new AuditRequest(url);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("urlWithProtocol")
            .WithMessage("Url must contain protocol*");
    }

    [Test]
    public void Defaults_AreUnset()
    {
        var request = new AuditRequest("http://example.com");

        request.MaxWaitForLoad.Should().BeNull();
        request.BlockedUrlPatterns.Should().BeNull();
        request.DisableStorageReset.Should().BeNull();
        request.DisableDeviceEmulation.Should().BeNull();
        request.EmulatedFormFactor.Should().BeNull();
        request.OnlyCategories.Should().BeNull();
        request.EnableLogging.Should().BeFalse();
    }
}
