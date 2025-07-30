namespace LighthousePlaywright.Net.Core;

internal sealed class BashTerminal : TerminalBase
{
    protected override string FileName => "which";

    internal Task<string> GetNodePathAsync()
    {
        return this.ExecuteAsync("node");
    }
}