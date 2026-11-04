using LighthousePlaywright.Net.Core;
using LighthousePlaywright.Net.Objects;

using System.IO;
using System.Text.RegularExpressions;

namespace LighthousePlaywright.Net;

/// <summary>
/// Class Lighthouse. This class cannot be inherited.
/// </summary>
public sealed partial class Lighthouse
{
    internal Options Options { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Lighthouse"/> class.
    /// </summary>
    public Lighthouse()
    {
       this.Options = new Options();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Lighthouse"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    public Lighthouse(Options options)
    {
        this.Options = options;
    }

    /// <summary>
    /// Run the lighthouse audit report.
    /// </summary>
    /// <param name="urlWithProtocol">The URL with protocol.</param>
    /// <returns>Task&lt;AuditResult&gt;.</returns>
    public Task<AuditResult> RunAsync(string urlWithProtocol)
    {
        return RunAsync(new AuditRequest(urlWithProtocol));
    }

    /// <summary>
    ///   Run the lighthouse audit report.
    /// </summary>
    /// <param name="request"></param>
    public Task<AuditResult> RunAsync(AuditRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return RunAfterCheckAsync(request);
    }

    private async Task<AuditResult> RunAfterCheckAsync(AuditRequest request)
    {
        string nodePath;

        if (OSystem.IsLinux)
        {
            var bashTerminal = new BashTerminal
            {
                EnableDebugging = request.EnableLogging
            };
            nodePath = await bashTerminal.GetNodePathAsync().ConfigureAwait(false);
        }
        else
        {
            var whereCmd = new WhereCmd
            {
                EnableDebugging = request.EnableLogging
            };
            nodePath = await whereCmd.GetNodePathAsync().ConfigureAwait(false);
        }

        if (string.IsNullOrEmpty(nodePath) || !File.Exists(nodePath))
        {
            throw new Exception(
                "Couldn't find NodeJs. Please, install NodeJs and make sure than PATH variable defined.");
        }

        var npm = new Npm(nodePath)
        {
            EnableDebugging = request.EnableLogging
        };
        var nodeModulesPath = await npm.GetGlobalNodeModulesPathAsync().ConfigureAwait(false);

        var sm = new ScriptMaker(Options);
        var content = sm.Produce(request, nodeModulesPath);
        if (!sm.Save(content))
        {
            throw new Exception($"Couldn't save JS script to %temp% directory. Path: {sm.TempFileName}");
        }

        try
        {
            var node = new Node
            {
                EnableDebugging = request.EnableLogging
            };
            var stdoutJson = await node.RunAsync(sm.TempFileName).ConfigureAwait(false);

            return ParseNodeOutput(stdoutJson);
        }
        catch (Exception ex) when (IsLighthouseMissing(ex.Message))
        {
            throw new Exception(LighthouseMissingMessage, ex);
        }
        finally
        {
            if (!npm.EnableDebugging)
            {
                sm.Delete();
            }
        }
    }

    internal const string LighthouseMissingMessage =
        "Lighthouse is not installed. Please, execute `npm install -g lighthouse playwright playwright-lighthouse` in console.";

    /// <summary>
    /// Extracts and parses the Lighthouse JSON report from the node process output,
    /// skipping anything logged before the JSON starts.
    /// </summary>
    internal static AuditResult ParseNodeOutput(string stdout)
    {
        var jsonStart = stdout?.IndexOf('{') ?? -1;
        if (jsonStart == -1)
        {
            throw new Exception(
                $"Lighthouse produced no JSON output. Raw output: {(string.IsNullOrEmpty(stdout) ? "<empty>" : stdout)}");
        }

        return AuditResult.Parse(stdout[jsonStart..]);
    }

    /// <summary>
    /// Determines whether a node error means one of the required global npm packages is missing.
    /// </summary>
    internal static bool IsLighthouseMissing(string message)
    {
        return !string.IsNullOrEmpty(message) && CannotFindModuleRegex().IsMatch(message);
    }

    [GeneratedRegex(@"Cannot find module[\s\S]+?node_modules[\\/](?:playwright-lighthouse|playwright|lighthouse)'")]
    private static partial Regex CannotFindModuleRegex();
}