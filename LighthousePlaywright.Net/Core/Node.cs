namespace LighthousePlaywright.Net.Core;

internal sealed class Node : TerminalBase
{
    protected override string FileName => "node";

    public Task<string> RunAsync(string jsFilePath)
    {
        return this.ExecuteAsync($"--harmony --unhandled-rejections=strict {jsFilePath}");
    }

    protected override void OnError(string message)
    {
        throw new Exception(message);
    }
}