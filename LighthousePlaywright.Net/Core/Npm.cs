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
    /// Gets the global node_modules directory (<c>npm root -g</c>) as an asynchronous operation.
    /// This resolves correctly on every OS, e.g. <c>%AppData%\npm\node_modules</c> on Windows
    /// and <c>{prefix}/lib/node_modules</c> on Linux.
    /// </summary>
    /// <returns>A Task&lt;System.String&gt; representing the asynchronous operation.</returns>
    /// <exception cref="Exception">Couldn't detect global node_modules path.</exception>
    internal async Task<string> GetGlobalNodeModulesPathAsync()
    {
        var rsp = await this.ExecuteAsync("root -g").ConfigureAwait(false);
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