namespace SplitWireTurkey.Tests;

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SplitWireTurkey.Services;
using Xunit;

public class ProcessCommandRunnerTests
{
    [Fact]
    public async Task RunAsync_large_output_completes_without_pipe_deadlock()
    {
        var runner = new ProcessCommandRunner();
        var result = await runner.RunAsync("cmd.exe", "/c for /L %i in (1,1,20000) do @echo line%i", TimeSpan.FromSeconds(30));

        Assert.False(result.TimedOut);
        Assert.Equal(0, result.ExitCode);
        Assert.Contains("line20000", result.StdOut);
    }

    [Fact]
    public async Task RunAsync_process_timeout_kills_tree_and_returns_timed_out()
    {
        var runner = new ProcessCommandRunner();
        var sw = Stopwatch.StartNew();
        var result = await runner.RunAsync("cmd.exe", "/c ping -n 30 127.0.0.1 >nul", TimeSpan.FromSeconds(1));
        sw.Stop();

        Assert.True(result.TimedOut);
        Assert.Equal(-1, result.ExitCode);
        Assert.True(sw.Elapsed < TimeSpan.FromSeconds(10), $"Elapsed was {sw.Elapsed.TotalSeconds}s, expected < 10s");
    }

    [Fact]
    public async Task RunAsync_non_existent_executable_returns_minus_one_and_does_not_throw()
    {
        var runner = new ProcessCommandRunner();
        var result = await runner.RunAsync("non_existent_executable_xyz_12345.exe", "", TimeSpan.FromSeconds(5));

        Assert.False(result.TimedOut);
        Assert.Equal(-1, result.ExitCode);
        Assert.False(string.IsNullOrEmpty(result.StdErr));
    }
}
