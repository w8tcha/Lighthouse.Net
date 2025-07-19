using System;

namespace Lighthouse.Net.Objects;

/// <summary>
/// Class NpmPackageVersion.
/// </summary>
internal class NpmPackageVersion
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NpmPackageVersion"/> class.
    /// </summary>
    /// <param name="version">The version.</param>
    /// <exception cref="ArgumentException">Invalid version</exception>
    public NpmPackageVersion(string version)
    {
        var arr = version.Split('.');
        if (arr.Length == 0)
        {
            throw new ArgumentException("Invalid version");
        }

        MajorVersion = int.Parse(arr[0]);
        MinorVersion = int.Parse(arr[1]);
        PatchVersion = int.Parse(arr[2]);
    }

    /// <summary>
    /// Gets the major version.
    /// </summary>
    /// <value>The major version.</value>
    public int MajorVersion { get; }

    /// <summary>
    /// Gets the minor version.
    /// </summary>
    /// <value>The minor version.</value>
    public int MinorVersion { get; }

    /// <summary>
    /// Gets the patch version.
    /// </summary>
    /// <value>The patch version.</value>
    public int PatchVersion { get; }
}