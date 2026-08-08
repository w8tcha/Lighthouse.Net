using System.IO;
using System.Linq;

namespace LighthousePlaywright.Net.Core;

internal sealed class BashTerminal : TerminalBase
{
    protected override string FileName => "which";

    internal async Task<string> GetNodePathAsync()
    {
        var rsp = await this.ExecuteAsync("node").ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(rsp))
        {
            return null;
        }

        return rsp
            .Split('\n')
            .Select(line => line.Trim())
            .FirstOrDefault(File.Exists);
    }
}