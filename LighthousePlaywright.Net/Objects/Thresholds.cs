namespace LighthousePlaywright.Net.Objects;

/// <summary>
/// Class Thresholds.
/// </summary>
public class Thresholds
{
    [JsonProperty(PropertyName = "performance")]
    public int Performance { get; set; } = 50;

    [JsonProperty(PropertyName = "accessibility")]
    public int Accessibility { get; set; } = 50;

    [JsonProperty(PropertyName = "best-practices")]
    public int BestPractices { get; set; } = 50;

    [JsonProperty(PropertyName = "seo")]
    public int Seo { get; set; } = 50;

    [JsonProperty(PropertyName = "pwa")]
    public int Pwa { get; set; } = 50;
}