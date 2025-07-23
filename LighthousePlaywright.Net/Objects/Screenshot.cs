namespace LighthousePlaywright.Net.Objects;

/// <summary>
/// Struct Screenshot
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="Screenshot"/> struct.
/// </remarks>
/// <param name="data">The data.</param>
public readonly struct Screenshot(string data)
{
    /// <summary>
    /// Gets the base64 data.
    /// </summary>
    /// <value>The base64 data.</value>
    public string Base64Data { get; } = data;
}