using System;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using Lighthouse.Net.Core;
using Lighthouse.Net.Objects;

namespace Lighthouse.Net;

/// <summary>
/// Class Lighthouse. This class cannot be inherited.
/// </summary>
public sealed partial class Lighthouse
{
    public Task<AuditResult> RunAsync(string urlWithProtocol)
    {
        return RunAsync(new AuditRequest(urlWithProtocol));
    }

    public Task<AuditResult> RunAsync(AuditRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return RunAfterCheckAsync(request);
    }

    private static async Task<AuditResult> RunAfterCheckAsync(AuditRequest request)
    {
        var cmd = new WhereCmd
        {
            EnableDebugging = request.EnableLogging
        };
        var nodePath = await cmd.GetNodePathAsync().ConfigureAwait(false);
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

        var version = await npm.GetLighthouseVersionAsync().ConfigureAwait(false);

        var sm = new ScriptMaker();
        var content = sm.Produce(request, npmPath, version);
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