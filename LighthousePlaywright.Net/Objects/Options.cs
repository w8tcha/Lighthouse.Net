namespace LighthousePlaywright.Net.Objects;

/// <summary>
/// Class Options.
/// </summary>
public class Options
{
    /// <summary>
    /// Gets or sets the reports.
    /// </summary>
    /// <value>The reports.</value>
    public Reports Reports { get; set; } = new();

    /// <summary>
    /// Gets or sets the thresholds.
    /// </summary>
    /// <value>The thresholds.</value>
    public Thresholds Thresholds { get; set; } = new();

    /// <summary>
    /// Gets or sets the chrome port.
    /// </summary>
    /// <value>The port.</value>
    public int Port { get; set; } = 9222;
}