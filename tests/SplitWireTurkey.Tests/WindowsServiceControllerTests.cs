namespace SplitWireTurkey.Tests;

using System;
using System.Threading.Tasks;
using SplitWireTurkey.Services;
using Xunit;

public class WindowsServiceControllerTests
{
    [Fact]
    public async Task StopAndDelete_when_service_not_installed_succeeds_and_issues_only_query()
    {
        var runner = new FakeCommandRunner();
        runner.EnqueueResponse("sc.exe", "query UnknownService", new CommandResult(1060, "The specified service does not exist as an installed service.", "", false));

        var controller = new WindowsServiceController(runner, delay: (t, ct) => Task.CompletedTask);

        var result = await controller.StopAndDeleteAsync("UnknownService");

        Assert.True(result.Success);
        Assert.Equal(ServiceState.NotInstalled, result.FinalState);
        Assert.Equal(new[] { "sc.exe|query UnknownService" }, runner.CallLog);
    }
}
