using System.IO;
using System.Linq;

namespace LighthousePlaywright.Net.Core;

internal sealed class WhereCmd : TerminalBase
{
    protected override string FileName => "where.exe";

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