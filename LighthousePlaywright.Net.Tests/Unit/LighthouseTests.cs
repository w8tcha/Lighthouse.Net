using System;
using System.Threading.Tasks;

using AwesomeAssertions;

using LighthousePlaywright.Net.Objects;

using NUnit.Framework;

namespace LighthousePlaywright.Net.Tests.Unit;

public class LighthouseTests
{
    [Test]
    public void DefaultConstructor_CreatesDefaultOptions()
    {
        new Lighthouse().Options.Should().NotBeNull();
    }

    [Test]
    public void OptionsConstructor_UsesGivenOptions()
    {
        var options = new Options { Port = 9876 };

        new Lighthouse(options).Options.Should().BeSameAs(options);
    }

    [Test]
    public async Task RunAsync_NullRequest_Throws()
    {
        var act = () => new Lighthouse().RunAsync((AuditRequest)null);

        await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("request");
    }

    [Test]
    public void ParseNodeOutput_SkipsLogLinesBeforeJson()
    {
        var result = Lighthouse.ParseNodeOutput(
            "Warning: something\nsome log\n{\"categories\":{\"performance\":{\"score\":0.8}}}");

        result.Performance.Should().Be(0.8m);
    }

    [TestCase(null, "<empty>")]
    [TestCase("", "<empty>")]
    [TestCase("no json here", "no json here")]
    public void ParseNodeOutput_WithoutJson_Throws(string stdout, string expectedRaw)
    {
        var act = () => Lighthouse.ParseNodeOutput(stdout);

        act.Should().Throw<Exception>()
            .WithMessage($"Lighthouse produced no JSON output. Raw output: {expectedRaw}");
    }

    [TestCase(@"Error: Cannot find module 'C:\Users\me\AppData\Roaming\npm\node_modules\playwright-lighthouse'")]
    [TestCase(@"Error: Cannot find module 'C:\Users\me\AppData\Roaming\npm\node_modules\playwright'")]
    [TestCase("Error: Cannot find module '/usr/lib/node_modules/lighthouse'\nRequire stack:\n- /tmp/x.js")]
    public void IsLighthouseMissing_MissingModule_ReturnsTrue(string message)
    {
        Lighthouse.IsLighthouseMissing(message).Should().BeTrue();
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("Error: Cannot find module 'C:\\npm\\node_modules\\left-pad'")]
    [TestCase("Error: net::ERR_NAME_NOT_RESOLVED")]
    public void IsLighthouseMissing_OtherErrors_ReturnsFalse(string message)
    {
        Lighthouse.IsLighthouseMissing(message).Should().BeFalse();
    }

    [Test]
    public async Task RunAsync_UrlWithoutProtocol_Throws()
    {
        var act = () => new Lighthouse().RunAsync("example.com");

        await act.Should().ThrowAsync<ArgumentException>();
    }
}
