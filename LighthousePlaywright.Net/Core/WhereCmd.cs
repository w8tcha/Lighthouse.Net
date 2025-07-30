namespace LighthousePlaywright.Net.Core;

internal sealed class WhereCmd : TerminalBase
{
    protected override string FileName => "where.exe";

    internal Task<string> GetNodePathAsync()
    {
        return this.ExecuteAsync("node");
    }
}