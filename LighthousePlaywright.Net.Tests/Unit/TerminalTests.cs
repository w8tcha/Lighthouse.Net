using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

using AwesomeAssertions;

using LighthousePlaywright.Net.Core;

using NUnit.Framework;

namespace LighthousePlaywright.Net.Tests.Unit;

/// <summary>
/// Tests for the process wrappers. These spawn real local processes (<c>dotnet</c>, <c>node</c>, <c>npm</c>)
/// but need neither network access nor a browser.
/// </summary>
public class TerminalTests
{
    private sealed class DotnetTerminal : TerminalBase
    {
        protected override string FileName => "dotnet";

        public string LastError { get; private set; }

        public Task<string> RunAsync(string arguments) => this.ExecuteAsync(arguments);

        protected override void OnError(string message) => this.LastError = message;
    }

    private sealed class DefaultErrorTerminal(string fileName) : TerminalBase
    {
        protected override string FileName => fileName;

        public Task<string> RunAsync(string arguments) => this.ExecuteAsync(arguments);
    }

    private string _tempDirectory;

    [SetUp]
    public void SetUp()
    {
        this._tempDirectory = Directory.CreateTempSubdirectory("lighthouse-net-tests-").FullName;
    }

    [TearDown]
    public void TearDown()
    {
        Directory.Delete(this._tempDirectory, true);
    }

    private string WriteScript(string content)
    {
        var path = Path.Combine(this._tempDirectory, $"{Guid.NewGuid():N}.js");
        File.WriteAllText(path, content);
        return path;
    }

    private static async Task<string> GetNodePathAsync()
    {
        var nodePath = OSystem.IsLinux
            ? await new BashTerminal().GetNodePathAsync()
            : await new WhereCmd().GetNodePathAsync();

        if (string.IsNullOrEmpty(nodePath))
        {
            Assert.Ignore("Node.js is not installed.");
        }

        return nodePath;
    }

    [Test]
    public async Task ExecuteAsync_Success_ReturnsStdout()
    {
        var terminal = new DotnetTerminal();

        var output = await terminal.RunAsync("--version");

        output.Should().MatchRegex(@"^\d+\.\d+\.\d+");
        terminal.LastError.Should().BeNull();
    }

    [Test]
    public async Task ExecuteAsync_NonZeroExitCode_CallsOnErrorWithStderr()
    {
        var terminal = new DotnetTerminal();

        await terminal.RunAsync("this-command-does-not-exist-lighthouse-net");

        terminal.LastError.Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    public async Task ExecuteAsync_DefaultOnError_DoesNotThrow()
    {
        var terminal = new DefaultErrorTerminal("dotnet");

        var act = () => terminal.RunAsync("this-command-does-not-exist-lighthouse-net");

        await act.Should().NotThrowAsync();
    }

    [Test]
    public async Task ExecuteAsync_MissingExecutable_Throws()
    {
        var terminal = new DefaultErrorTerminal($"missing-{Guid.NewGuid():N}");

        var act = () => terminal.RunAsync(string.Empty);

        await act.Should().ThrowAsync<Win32Exception>();
    }

    [Test]
    public async Task ExecuteAsync_WithDebugging_StillReturnsOutput()
    {
        var terminal = new DotnetTerminal { EnableDebugging = true };

        var output = await terminal.RunAsync("--version");

        output.Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    public async Task GetNodePathAsync_FindsExistingNodeExecutable()
    {
        var nodePath = await GetNodePathAsync();

        File.Exists(nodePath).Should().BeTrue();
        Path.GetFileNameWithoutExtension(nodePath).Should().BeEquivalentTo("node");
    }

    [Test]
    public async Task Node_RunAsync_JoinsMultilineStdoutAndSkipsEmptyLines()
    {
        await GetNodePathAsync();
        var script = this.WriteScript("console.log('first'); console.log(''); console.log('second');");

        var output = await new Node().RunAsync(script);

        output.Should().Be("first\nsecond");
    }

    [Test]
    public async Task Node_RunAsync_ScriptPathWithSpaces_Works()
    {
        await GetNodePathAsync();
        var directory = Directory.CreateDirectory(Path.Combine(this._tempDirectory, "with space")).FullName;
        var script = Path.Combine(directory, "script.js");
        await File.WriteAllTextAsync(script, "console.log('ok');");

        var output = await new Node().RunAsync(script);

        output.Should().Be("ok");
    }

    [Test]
    public async Task Node_RunAsync_FailingScript_ThrowsWithStderr()
    {
        await GetNodePathAsync();
        var script = this.WriteScript("console.error('boom'); process.exit(3);");

        var act = () => new Node().RunAsync(script);

        await act.Should().ThrowAsync<Exception>().WithMessage("boom");
    }

    [Test]
    public async Task Node_RunAsync_FailingScriptWithoutStderr_ThrowsWithExitCode()
    {
        await GetNodePathAsync();
        var script = this.WriteScript("process.exit(7);");

        var act = () => new Node().RunAsync(script);

        await act.Should().ThrowAsync<Exception>().WithMessage("Process exited with code 7.");
    }

    [Test]
    public async Task Npm_GetGlobalNodeModulesPathAsync_ReturnsExistingDirectory()
    {
        var nodePath = await GetNodePathAsync();

        var nodeModules = await new Npm(nodePath).GetGlobalNodeModulesPathAsync();

        nodeModules.Should().Be(nodeModules.Trim());
        Path.GetFileName(nodeModules).Should().Be("node_modules");
        Directory.Exists(nodeModules).Should().BeTrue();
    }

    [Test]
    public async Task Npm_WithInvalidNodeDirectory_Throws()
    {
        var npm = new Npm(Path.Combine(this._tempDirectory, "node.exe"));

        var act = () => npm.GetGlobalNodeModulesPathAsync();

        await act.Should().ThrowAsync<Exception>();
    }

    [Test]
    public void Logger_Append_ReturnsTrue()
    {
        var logger = new Logger("lighthouse-net-tests");

        logger.Append("line 1").Should().BeTrue();
        logger.Append("line 2").Should().BeTrue();
    }
}
