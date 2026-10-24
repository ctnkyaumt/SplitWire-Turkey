namespace SplitWireTurkey.Services;

public sealed class WindowsServiceController
{
    public WindowsServiceController(ICommandRunner runner,
        Func<TimeSpan, CancellationToken, Task> delay = null,
        TimeSpan? stopTimeout = null,
        TimeSpan? pollInterval = null,
        TimeSpan? commandTimeout = null)
    {
    }

    public Task<ServiceState> QueryAsync(string serviceName, CancellationToken ct = default) => throw new NotImplementedException();

    public Task<IReadOnlyDictionary<string, ServiceState>> QueryManyAsync(IEnumerable<string> serviceNames, CancellationToken ct = default) => throw new NotImplementedException();

    public Task<bool> IsInstalledAsync(string serviceName, CancellationToken ct = default) => throw new NotImplementedException();

    public Task<ServiceOperationResult> StartAsync(string serviceName, CancellationToken ct = default) => throw new NotImplementedException();

    public Task<ServiceOperationResult> StopAsync(string serviceName, IProgress<string> log = null, CancellationToken ct = default) => throw new NotImplementedException();

    public Task<ServiceOperationResult> StopAndDeleteAsync(string serviceName, IProgress<string> log = null, CancellationToken ct = default) => throw new NotImplementedException();

    public Task<IReadOnlyList<ServiceOperationResult>> StopAndDeleteManyAsync(IEnumerable<string> serviceNames, IProgress<string> log = null, CancellationToken ct = default) => throw new NotImplementedException();
}
