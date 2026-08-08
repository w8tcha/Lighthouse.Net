using System.Net;
using System.Net.Sockets;

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
    /// Gets or sets the chrome port. Defaults to a free ephemeral port picked at construction time,
    /// so that concurrent audits (e.g. multiple <see cref="Lighthouse"/> instances running at once)
    /// don't collide on the same Chrome DevTools Protocol port. This is best-effort: the port is
    /// released back to the OS immediately after being chosen, so under heavy concurrent load another
    /// process could still claim it before Chrome actually launches. Set this explicitly if you need
    /// a guaranteed port.
    /// </summary>
    /// <value>The port.</value>
    public int Port { get; set; } = GetFreePort();

    private static int GetFreePort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }
}