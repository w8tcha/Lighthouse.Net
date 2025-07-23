namespace Lighthouse.Net.Objects;

/// <summary>
/// Struct Screenshot
/// </summary>
public struct Screenshot
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Screenshot"/> struct.
    /// </summary>
    /// <param name="data">The data.</param>
    public Screenshot(string data)
    {
        this.Base64Data = data;
    }

    /// <summary>
    /// Gets the base64 data.
    /// </summary>
    /// <value>The base64 data.</value>
    public string Base64Data { get; }
}