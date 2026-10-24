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
}
