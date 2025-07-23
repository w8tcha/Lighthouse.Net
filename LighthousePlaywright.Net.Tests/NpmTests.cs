using AwesomeAssertions;

using LighthousePlaywright.Net.Objects;

using System.Threading.Tasks;

using Xunit;

namespace LighthousePlaywright.Net.Tests;

public class NpmTests
{
    [Fact]
    public async Task NpmExistTest()
    {
        var lh = new Lighthouse();
        var res = await lh.RunAsync("http://example.com");

        res.Should().NotBeNull();
        res.Performance.Should().NotBeNull();
        (res.Performance > 0.5m).Should().BeTrue();

        res.Accessibility.Should().NotBeNull();
        (res.Accessibility > 0.5m).Should().BeTrue();

        res.BestPractices.Should().NotBeNull();
        (res.BestPractices > 0.5m).Should().BeTrue();

        res.Seo.Should().NotBeNull();
        (res.Seo > 0.2m).Should().BeTrue();
    }

    [Fact]
    public async Task OnlyCategoriesTest()
    {
        var lh = new Lighthouse();
        var ar = new AuditRequest("http://example.com")
        {
            OnlyCategories =
            [
                Category.Performance
            ],
            EnableLogging = true
        };
        var res = await lh.RunAsync(ar);

        res.Should().NotBeNull();
        res.Performance.Should().NotBeNull();
        (res.Performance > 0.5m).Should().BeTrue();
        res.Accessibility.Should().BeNull();
    }

    [Fact]
    public async Task ScreenShots()
    {
        var lh = new Lighthouse();
        var res = await lh.RunAsync("http://example.com");

        res.Should().NotBeNull();
        res.FinalScreenshot.Should().NotBeNull();
        string.IsNullOrWhiteSpace(res.FinalScreenshot.Base64Data).Should().BeFalse();

        res.Thumbnails.Should().NotBeNull();
        (res.Thumbnails.Count == 0).Should().BeFalse();
        string.IsNullOrWhiteSpace(res.Thumbnails[0].Base64Data).Should().BeFalse();
    }

    [Fact]
    public async Task FormFactorTest()
    {
        var lh = new Lighthouse();
        var ar = new AuditRequest("http://example.com")
        {
            EmulatedFormFactor = AuditRequest.FormFactor.Desktop
        };

        var res = await lh.RunAsync(ar);

        res.Should().NotBeNull();
    }
}