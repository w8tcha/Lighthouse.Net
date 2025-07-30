namespace LighthousePlaywright.Net.Core;

/// <summary>
/// Class OSystem.
/// </summary>
public static class OSystem
{
    /// <summary>
    /// Gets a value indicating whether this instance is running linux.
    /// </summary>
    /// <value><c>true</c> if this instance is running linux; otherwise, <c>false</c>.</value>
    public static bool IsLinux
    {
        get
        {
            var platform = (int)Environment.OSVersion.Platform;
            return platform is 4 or 6 or 128;
        }
    }
}