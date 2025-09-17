namespace LighthousePlaywright.Net.Objects;

/// <summary>
/// Class Thresholds.
/// </summary>
public class Thresholds
{
    /// <summary>
    /// Gets or sets the performance.
    /// </summary>
    /// <value>The performance.</value>
    [JsonProperty(PropertyName = "performance")]
    public int Performance { get; set; } = 50;

    /// <summary>
    /// Gets or sets the accessibility.
    /// </summary>
    /// <value>The accessibility.</value>
    [JsonProperty(PropertyName = "accessibility")]
    public int Accessibility { get; set; } = 50;

    /// <summary>
    /// Gets or sets the best practices.
    /// </summary>
    /// <value>The best practices.</value>
    [JsonProperty(PropertyName = "best-practices")]
    public int BestPractices { get; set; } = 50;

    /// <summary>
    /// Gets or sets the seo.
    /// </summary>
    /// <value>The seo.</value>
    [JsonProperty(PropertyName = "seo")]
    public int Seo { get; set; } = 50;

    /// <summary>
    /// Gets or sets the pwa.
    /// </summary>
    /// <value>The pwa.</value>
    [JsonProperty(PropertyName = "pwa")]
    public int Pwa { get; set; } = 50;
}