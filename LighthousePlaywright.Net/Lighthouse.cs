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
    internal Options Options { get; private set; }

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

    public Task<AuditResult> RunAsync(string urlWithProtocol)
    {
        return RunAsync(new AuditRequest(urlWithProtocol));
    }

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
        var npmPath = await npm.GetNpmPathAsync().ConfigureAwait(false);

        var sm = new ScriptMaker(Options);
        var content = sm.Produce(request, npmPath);
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

            return AuditResult.Parse(stdoutJson[stdoutJson.IndexOf('{')..]);
        }
        catch (Exception ex)
        {
            if (!string.IsNullOrEmpty(ex.Message) && CannotFindModuleRegex().IsMatch(ex.Message))
            {
                throw new Exception(
                    "Lighthouse is not installed. Please, execute `npm install -g lighthouse` in console.");
            }

            throw;
        }
        finally
        {
            if (!npm.EnableDebugging)
            {
                sm.Delete();
            }
        }
    }

    [GeneratedRegex(@"Cannot find module[\s\S]+?node_modules\\lighthouse'")]
    private static partial Regex CannotFindModuleRegex();
}