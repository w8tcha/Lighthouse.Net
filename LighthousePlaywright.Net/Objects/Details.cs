namespace LighthousePlaywright.Net.Objects;

/// <summary>
/// Class Details. This class cannot be inherited.
/// </summary>
public sealed class Details
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the score.
    /// </summary>
    /// <value>The score.</value>
    public decimal? Score { get; set; }

    /// <summary>
    /// Gets or sets the numeric value.
    /// </summary>
    /// <value>The numeric value.</value>
    public decimal? NumericValue { get; set; }
}