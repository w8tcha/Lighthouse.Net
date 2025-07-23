using Lighthouse.Net.Objects;

using System;
using System.Threading.Tasks;

namespace Lighthouse.Net.Core;

/// <summary>
/// Class Npm. This class cannot be inherited.
/// </summary>
internal sealed class Npm(string nodePath) : TerminalBase
{
    /// <summary>
    /// Gets the name of the file.
    /// </summary>
    /// <value>The name of the file.</value>
    protected override string FileName { get; } = nodePath.Replace("node.exe", "npm.cmd");

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
    /// Get lighthouse version as an asynchronous operation.
    /// </summary>
    /// <returns>A Task&lt;NpmPackageVersion&gt; representing the asynchronous operation.</returns>
    /// <exception cref="Exception">Couldn't detect lighthouse version.</exception>
    internal async Task<NpmPackageVersion> GetLighthouseVersionAsync()
    {
        var rsp = await this.ExecuteAsync("ls --depth=0 -global lighthouse");
        var index = !string.IsNullOrEmpty(rsp) ? rsp.IndexOf('@') : -1;
        if (rsp == null || index == -1)
        {
            throw new Exception("Couldn't detect lighthouse version.");
        }

        return new NpmPackageVersion(rsp[(index + 1)..].Trim());
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