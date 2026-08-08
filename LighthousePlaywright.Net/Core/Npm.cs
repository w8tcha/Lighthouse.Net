using System.IO;

namespace LighthousePlaywright.Net.Core;

/// <summary>
/// Class Npm. This class cannot be inherited.
/// </summary>
internal sealed class Npm(string nodePath) : TerminalBase
{
    /// <summary>
    /// Gets the name of the file.
    /// </summary>
    /// <value>The name of the file.</value>
    protected override string FileName { get; } = GetNpmPath(nodePath);

    private static string GetNpmPath(string nodePath)
    {
        var directory = Path.GetDirectoryName(nodePath) ?? string.Empty;
        var npmFileName = OSystem.IsLinux ? "npm" : "npm.cmd";
        return Path.Combine(directory, npmFileName);
    }

    /// <summary>
    /// Get NPM path as an asynchronous operation.
    /// </summary>
    /// <returns>A Task&lt;System.String&gt; representing the asynchronous operation.</returns>
    /// <exception cref="Exception">Couldn't detect global node_modules path.</exception>
    internal async Task<string> GetNpmPathAsync()
    {
        var rsp = await this.ExecuteAsync("config get prefix");
        if (string.IsNullOrEmpty(rsp))
        {
            throw new Exception("Couldn't detect global node_modules path.");
        }

        return rsp.Trim();
    }

    /// <summary>
    /// Called when [error].
    /// </summary>
    /// <param name="message">The message.</param>
    /// <exception cref="Exception"></exception>
    protected override void OnError(string message)
    {
        throw new Exception(message);
    }
}