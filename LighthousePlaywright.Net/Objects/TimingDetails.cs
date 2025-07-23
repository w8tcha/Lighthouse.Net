namespace LighthousePlaywright.Net.Objects;

/// <summary>
/// Class TimingDetails. This class cannot be inherited.
/// </summary>
public sealed class TimingDetails
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the start time.
    /// </summary>
    /// <value>The start time.</value>
    public decimal? StartTime { get; set; }

    /// <summary>
    /// Gets or sets the duration.
    /// </summary>
    /// <value>The duration.</value>
    public decimal? Duration { get; set; }
}