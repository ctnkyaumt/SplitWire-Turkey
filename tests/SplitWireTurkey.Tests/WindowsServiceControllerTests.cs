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

    [Fact]
    public async Task StopAndDelete_when_service_running_stops_polls_until_stopped_and_deletes()
    {
        var runner = new FakeCommandRunner();
        // 1st query: RUNNING
        runner.EnqueueResponse("sc.exe", "query RunningService", new CommandResult(0, "STATE : 4 RUNNING", "", false));
        // stop command
        runner.EnqueueResponse("sc.exe", "stop RunningService", new CommandResult(0, "", "", false));
        // 2nd query (poll 1): STOP_PENDING
        runner.EnqueueResponse("sc.exe", "query RunningService", new CommandResult(0, "STATE : 3 STOP_PENDING", "", false));
        // 3rd query (poll 2): STOPPED
        runner.EnqueueResponse("sc.exe", "query RunningService", new CommandResult(0, "STATE : 1 STOPPED", "", false));
        // delete command
        runner.EnqueueResponse("sc.exe", "delete RunningService", new CommandResult(0, "", "", false));

        var controller = new WindowsServiceController(runner, delay: (t, ct) => Task.CompletedTask);

        var result = await controller.StopAndDeleteAsync("RunningService");

        Assert.True(result.Success);
        Assert.Equal(ServiceState.NotInstalled, result.FinalState);
        Assert.Equal(new[]
        {
            "sc.exe|query RunningService",
            "sc.exe|stop RunningService",
            "sc.exe|query RunningService",
            "sc.exe|query RunningService",
            "sc.exe|delete RunningService"
        }, runner.CallLog);
    }

    [Fact]
    public async Task StopAndDelete_when_service_never_stops_times_out_polling_then_deletes()
    {
        // StopAsync alone returns Success=false
        var runner1 = new FakeCommandRunner();
        runner1.EnqueueResponse("sc.exe", "query StuckService", new CommandResult(0, "STATE : 4 RUNNING", "", false));
        runner1.EnqueueResponse("sc.exe", "stop StuckService", new CommandResult(0, "", "", false));

        var controller1 = new WindowsServiceController(runner1,
            delay: (t, ct) => Task.CompletedTask,
            stopTimeout: TimeSpan.FromMilliseconds(1000),
            pollInterval: TimeSpan.FromMilliseconds(500));

        var stopAlone = await controller1.StopAsync("StuckService");
        Assert.False(stopAlone.Success);
        Assert.Equal(ServiceState.Running, stopAlone.FinalState);

        // StopAndDeleteAsync polls then still calls delete; success depends on delete exit code
        var runner2 = new FakeCommandRunner();
        runner2.EnqueueResponse("sc.exe", "query StuckService", new CommandResult(0, "STATE : 4 RUNNING", "", false));
        runner2.EnqueueResponse("sc.exe", "stop StuckService", new CommandResult(0, "", "", false));
        runner2.EnqueueResponse("sc.exe", "delete StuckService", new CommandResult(0, "", "", false));

        var controller2 = new WindowsServiceController(runner2,
            delay: (t, ct) => Task.CompletedTask,
            stopTimeout: TimeSpan.FromMilliseconds(1000),
            pollInterval: TimeSpan.FromMilliseconds(500));

        var result = await controller2.StopAndDeleteAsync("StuckService");
        Assert.True(result.Success);
        Assert.Equal(ServiceState.NotInstalled, result.FinalState);
        Assert.Equal(new[]
        {
            "sc.exe|query StuckService",
            "sc.exe|stop StuckService",
            "sc.exe|query StuckService",
            "sc.exe|query StuckService",
            "sc.exe|delete StuckService"
        }, runner2.CallLog);
    }

    [Fact]
    public async Task StopAndDelete_when_delete_returns_1072_treats_as_success()
    {
        var runner = new FakeCommandRunner();
        runner.EnqueueResponse("sc.exe", "query PendingDeleteService", new CommandResult(0, "STATE : 1 STOPPED", "", false));
        runner.EnqueueResponse("sc.exe", "delete PendingDeleteService", new CommandResult(1072, "The specified service has been marked for deletion.", "", false));

        var controller = new WindowsServiceController(runner, delay: (t, ct) => Task.CompletedTask);

        var result = await controller.StopAndDeleteAsync("PendingDeleteService");

        Assert.True(result.Success);
        Assert.Equal(ServiceState.NotInstalled, result.FinalState);
    }

    [Fact]
    public async Task StopAndDeleteMany_processes_in_catalog_order_and_continues_after_failure()
    {
        var runner = new FakeCommandRunner();
        // ByeDPI (8th in catalog) and zapret (1st in catalog) passed in reverse order
        runner.EnqueueResponse("sc.exe", "query zapret", new CommandResult(0, "STATE : 1 STOPPED", "", false));
        runner.EnqueueResponse("sc.exe", "delete zapret", new CommandResult(5, "Access is denied.", "", false));

        runner.EnqueueResponse("sc.exe", "query ByeDPI", new CommandResult(0, "STATE : 1 STOPPED", "", false));
        runner.EnqueueResponse("sc.exe", "delete ByeDPI", new CommandResult(0, "", "", false));

        var controller = new WindowsServiceController(runner, delay: (t, ct) => Task.CompletedTask);

        var results = await controller.StopAndDeleteManyAsync(new[] { "ByeDPI", "zapret" });

        Assert.Equal(2, results.Count);
        Assert.Equal("zapret", results[0].ServiceName);
        Assert.False(results[0].Success);

        Assert.Equal("ByeDPI", results[1].ServiceName);
        Assert.True(results[1].Success);

        Assert.Equal(new[]
        {
            "sc.exe|query zapret",
            "sc.exe|delete zapret",
            "sc.exe|query ByeDPI",
            "sc.exe|delete ByeDPI"
        }, runner.CallLog);
    }

    [Fact]
    public async Task StartAsync_treats_1056_already_running_as_success()
    {
        var runner = new FakeCommandRunner();
        runner.EnqueueResponse("sc.exe", "start AlreadyRunningService", new CommandResult(1056, "An instance of the service is already running.", "", false));
        runner.EnqueueResponse("sc.exe", "query AlreadyRunningService", new CommandResult(0, "STATE : 4 RUNNING", "", false));

        var controller = new WindowsServiceController(runner, delay: (t, ct) => Task.CompletedTask);

        var result = await controller.StartAsync("AlreadyRunningService");

        Assert.True(result.Success);
        Assert.Equal(ServiceState.Running, result.FinalState);
    }

    [Fact]
    public async Task QueryManyAsync_returns_case_insensitive_dictionary()
    {
        var runner = new FakeCommandRunner();
        runner.EnqueueResponse("sc.exe", "query Goodbyedpi", new CommandResult(0, "STATE : 4 RUNNING", "", false));

        var controller = new WindowsServiceController(runner, delay: (t, ct) => Task.CompletedTask);
        var dict = await controller.QueryManyAsync(new[] { "Goodbyedpi" });

        Assert.Equal(ServiceState.Running, dict["goodbyedpi"]);
        Assert.Equal(ServiceState.Running, dict["GOODBYEDPI"]);
    }

    [Fact]
    public async Task IsInstalledAsync_returns_true_only_when_installed_and_not_unknown()
    {
        var runner = new FakeCommandRunner();
        runner.EnqueueResponse("sc.exe", "query SvcInstalled", new CommandResult(0, "STATE : 1 STOPPED", "", false));
        runner.EnqueueResponse("sc.exe", "query SvcMissing", new CommandResult(1060, "", "", false));
        runner.EnqueueResponse("sc.exe", "query SvcUnknown", new CommandResult(1, "Access denied", "", false));

        var controller = new WindowsServiceController(runner, delay: (t, ct) => Task.CompletedTask);

        Assert.True(await controller.IsInstalledAsync("SvcInstalled"));
        Assert.False(await controller.IsInstalledAsync("SvcMissing"));
        Assert.False(await controller.IsInstalledAsync("SvcUnknown"));
    }
}
