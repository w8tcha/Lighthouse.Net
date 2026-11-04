using System.Linq;

using AwesomeAssertions;

using LighthousePlaywright.Net.Objects;

using Newtonsoft.Json;

using NUnit.Framework;

namespace LighthousePlaywright.Net.Tests.Unit;

public class AuditResultTests
{
    private const string FullReport = """
        {
          "environment": { "benchmarkIndex": 1234.5 },
          "categories": {
            "performance": { "score": 0.91 },
            "accessibility": { "score": 0.88 },
            "best-practices": { "score": 1 },
            "seo": { "score": 0.7 },
            "pwa": { "score": 0.3 }
          },
          "audits": {
            "first-contentful-paint": { "score": 0.99, "numericValue": 812.4 },
            "final-screenshot": { "score": null, "details": { "data": "data:image/jpeg;base64,FINAL" } },
            "screenshot-thumbnails": {
              "score": null,
              "details": { "items": [ { "data": "THUMB1" }, { "data": "" }, { "data": "THUMB2" } ] }
            }
          },
          "timing": {
            "entries": [
              { "name": "lh:runner:run", "startTime": 10.5, "duration": 4000.25 },
              { "name": "lh:runner:gather", "startTime": 11, "duration": 2500 }
            ],
            "total": 4100.75
          }
        }
        """;

    [TestCase(null)]
    [TestCase("")]
    public void Parse_NullOrEmpty_ReturnsNull(string json)
    {
        AuditResult.Parse(json).Should().BeNull();
    }

    [Test]
    public void Parse_InvalidJson_Throws()
    {
        var act = () => AuditResult.Parse("not json");

        act.Should().Throw<JsonReaderException>();
    }

    [Test]
    public void Parse_FullReport_MapsCategoryScores()
    {
        var result = AuditResult.Parse(FullReport);

        result.Performance.Should().Be(0.91m);
        result.Accessibility.Should().Be(0.88m);
        result.BestPractices.Should().Be(1m);
        result.Seo.Should().Be(0.7m);
        result.Pwa.Should().Be(0.3m);
        result.BenchmarkIndex.Should().Be(1234.5m);
    }

    [Test]
    public void Parse_FullReport_MapsAuditDetails()
    {
        var result = AuditResult.Parse(FullReport);

        result.Details.Should().HaveCount(3);
        result.Details.Should().ContainSingle(d => d.Name == "first-contentful-paint")
            .Which.Should().BeEquivalentTo(new Details
            {
                Name = "first-contentful-paint", Score = 0.99m, NumericValue = 812.4m
            });
        result.Details.Should().ContainSingle(d => d.Name == "final-screenshot")
            .Which.Score.Should().BeNull();
    }

    [Test]
    public void Parse_FullReport_MapsScreenshotsAndSkipsEmptyThumbnails()
    {
        var result = AuditResult.Parse(FullReport);

        result.FinalScreenshot.Base64Data.Should().Be("data:image/jpeg;base64,FINAL");
        result.Thumbnails.Select(t => t.Base64Data).Should().Equal("THUMB1", "THUMB2");
    }

    [Test]
    public void Parse_FullReport_MapsTiming()
    {
        var result = AuditResult.Parse(FullReport);

        result.TimingDetails.Should().HaveCount(2);
        result.TimingDetails[0].Should().BeEquivalentTo(new TimingDetails
        {
            Name = "lh:runner:run", StartTime = 10.5m, Duration = 4000.25m
        });
        result.TotalDuration.Should().Be(4100.75m);
    }

    [Test]
    public void Parse_EmptyObject_ReturnsEmptyResult()
    {
        var result = AuditResult.Parse("{}");

        result.Should().NotBeNull();
        result.Performance.Should().BeNull();
        result.Accessibility.Should().BeNull();
        result.BestPractices.Should().BeNull();
        result.Seo.Should().BeNull();
        result.Pwa.Should().BeNull();
        result.BenchmarkIndex.Should().BeNull();
        result.TotalDuration.Should().BeNull();
        result.Details.Should().BeEmpty();
        result.TimingDetails.Should().BeEmpty();
        result.Thumbnails.Should().BeEmpty();
        result.FinalScreenshot.Base64Data.Should().BeNull();
    }

    [Test]
    public void Parse_OnlySomeCategories_LeavesOthersNull()
    {
        var result = AuditResult.Parse("""{ "categories": { "performance": { "score": 0.5 } } }""");

        result.Performance.Should().Be(0.5m);
        result.Accessibility.Should().BeNull();
        result.BestPractices.Should().BeNull();
    }

    [Test]
    public void Parse_ScreenshotAuditsWithoutData_LeaveScreenshotsEmpty()
    {
        const string json = """
            {
              "audits": {
                "final-screenshot": { "details": { } },
                "screenshot-thumbnails": { "details": { "items": [] } }
              }
            }
            """;

        var result = AuditResult.Parse(json);

        result.FinalScreenshot.Base64Data.Should().BeNull();
        result.Thumbnails.Should().BeEmpty();
        result.Details.Should().HaveCount(2);
    }

    [Test]
    public void Parse_TimingWithoutEntries_DoesNotSetTotalDuration()
    {
        var result = AuditResult.Parse("""{ "timing": { "total": 99 } }""");

        result.TimingDetails.Should().BeEmpty();
        result.TotalDuration.Should().BeNull();
    }
}
