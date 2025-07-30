namespace LighthousePlaywright.Net.Objects;

/// <summary>
/// Class ReportFormats.
/// </summary>
public class Reports
{
    /// <summary>
    /// Gets or sets the formats.
    /// </summary>
    /// <value>The formats.</value>
    [JsonProperty(PropertyName = "formats")]
    public Formats Formats { get; set; } = new();

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; } = $"lighthouse-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

    /// <summary>
    /// Gets or sets the directory.
    /// </summary>
    /// <value>The directory.</value>
    [JsonProperty(PropertyName = "directory")]
    public string Directory { get; set; } = "lighthouse";
}

/// <summary>
/// Class Formats.
/// </summary>
public class Formats
{
    /// <summary>
    /// Gets or sets a value indicating whether html reports are created.
    /// </summary>
    /// <value><c>true</c> if HTML; otherwise, <c>false</c>.</value>
    [JsonProperty(PropertyName = "html")]
    public bool Html { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether json reports are created.
    /// </summary>
    /// <value><c>true</c> if json; otherwise, <c>false</c>.</value>
    [JsonProperty(PropertyName = "json")]
    public bool Json { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether csv reports are created.
    /// </summary>
    /// <value><c>true</c> if CSV; otherwise, <c>false</c>.</value>
    [JsonProperty(PropertyName = "csv")]
    public bool Csv { get; set; }
}