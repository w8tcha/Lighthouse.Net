using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json.Linq;

namespace LighthousePlaywright.Net.Objects;

/// <summary>
/// Lighthouse Audit Result
/// </summary>
public sealed class AuditResult
{
    /// <summary>
    /// Performance scope
    /// </summary>
    public decimal? Performance { get; set; }

    /// <summary>
    /// Accessibility scope
    /// </summary>
    public decimal? Accessibility { get; set; }

    /// <summary>
    /// Best practices scope
    /// </summary>
    public decimal? BestPractices { get; set; }

    /// <summary>
    /// SEO scope
    /// </summary>
    public decimal? Seo { get; set; }

    /// <summary>
    /// Progressive Web App scope
    /// </summary>
    public decimal? Pwa { get; set; }

    /// <summary>
    /// Gets or sets the index of the benchmark.
    /// </summary>
    /// <value>The index of the benchmark.</value>
    public decimal? BenchmarkIndex { get; set; }

    /// <summary>
    /// Gets or sets the details.
    /// </summary>
    /// <value>The details.</value>
    public List<Details> Details { get; set; }

    /// <summary>
    /// Gets or sets the timing details.
    /// </summary>
    /// <value>The timing details.</value>
    public List<TimingDetails> TimingDetails { get; set; }

    /// <summary>
    /// Gets or sets the total duration.
    /// </summary>
    /// <value>The total duration.</value>
    public decimal? TotalDuration { get; set; }

    /// <summary>
    /// Gets or sets the final screenshot.
    /// </summary>
    /// <value>The final screenshot.</value>
    public Screenshot FinalScreenshot { get; set; }

    /// <summary>
    /// Gets or sets the thumbnails.
    /// </summary>
    /// <value>The thumbnails.</value>
    public IReadOnlyList<Screenshot> Thumbnails { get; set; }

    /// <summary>
    /// Parse the Audit Result.
    /// </summary>
    /// <param name="json">The json.</param>
    /// <returns>LighthousePlaywright.Net.Objects.AuditResult.</returns>
    public static AuditResult Parse(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        dynamic jObj = JObject.Parse(json);

        var a = new AuditResult
        {
            Details = [],
            TimingDetails = [],
            BenchmarkIndex = jObj.environment?.benchmarkIndex,
            Performance = jObj.categories?.performance?.score,
            Accessibility = jObj.categories?.accessibility?.score,
            BestPractices = jObj.categories?["best-practices"]?.score,
            Seo = jObj.categories?.seo?.score,
            Pwa = jObj.categories?.pwa?.score
        };


        if (jObj.audits is JObject audits)
        {
            foreach (var audit in audits)
            {
                dynamic val = audit.Value;
                a.Details.Add(new Details
                {
                    Name = audit.Key,
                    Score = val.score,
                    NumericValue = val.numericValue
                });

                switch (audit.Key)
                {
                    case "final-screenshot":
                    {
                        var base64 = val.details?.data?.ToString();
                        if (!string.IsNullOrEmpty(base64))
                        {
                            a.FinalScreenshot = new Screenshot(base64);
                        }

                        break;
                    }
                    case "screenshot-thumbnails" when val.details?.items is JArray { HasValues: true } items:
                    {
                        var screenShots =
                            (from item in items
                                select ((dynamic)item).data?.ToString()
                                into base64
                                where !string.IsNullOrEmpty(base64)
                                select new Screenshot(base64)).ToList();
                        a.Thumbnails = screenShots;
                        break;
                    }
                }
            }
        }

        if (jObj.timing?.entries is not JArray timingDetails)
        {
            return a;
        }

        foreach (var timingDetail in timingDetails)
        {
            dynamic val = timingDetail;
            a.TimingDetails.Add(new TimingDetails
            {
                Name = val?.name,
                StartTime = val?.startTime,
                Duration = val?.duration
            });
        }

        a.TotalDuration = jObj.timing?.total;

        return a;
    }
}