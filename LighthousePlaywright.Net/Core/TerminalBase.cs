using System.Diagnostics;
using System.Text;

namespace LighthousePlaywright.Net.Core;

/// <summary>
/// Class TerminalBase.
/// </summary>
internal abstract class TerminalBase
{
    protected abstract string FileName { get; }

    protected async Task<string> ExecuteAsync(string arguments)
    {
        Logger logger = null;
        if (this.EnableDebugging)
        {
            logger = new Logger("lighthouse-net-output-console");
        }

        var processInfo = new ProcessStartInfo
        {
            FileName = this.FileName,
            Arguments = arguments,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            StandardOutputEncoding = Encoding.UTF8,
            WindowStyle = ProcessWindowStyle.Hidden
        };
        using var process = Process.Start(processInfo);
        if (process == null)
        {
            return await Task.FromResult<string>(null);
        }

        StringBuilder sb = new(), sbError = new();

        logger?.Append($"Command: {this.FileName} {arguments}\r\n\r\n");

        process.OutputDataReceived += (_, args) =>
        {
            if (string.IsNullOrEmpty(args.Data))
            {
                return;
            }

            if (sb.Length > 0)
            {
                sb.Append('\n');
            }

            sb.Append(args.Data);
            logger?.Append(args.Data);
        };
        process.ErrorDataReceived += (_, args) =>
        {
            if (string.IsNullOrEmpty(args.Data))
            {
                return;
            }

            if (sbError.Length > 0)
            {
                sbError.Append('\n');
            }

            sbError.Append(args.Data);
            logger?.Append(args.Data);
        };
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync();

        var output = sb.ToString();
        var err = sbError.ToString();
        if (process.ExitCode != 0)
        {
            this.OnError(string.IsNullOrEmpty(err) ? $"Process exited with code {process.ExitCode}." : err);
        }

        return output;
    }

    protected virtual void OnError(string message)
    {
    }

    internal bool EnableDebugging { get; set; }
}