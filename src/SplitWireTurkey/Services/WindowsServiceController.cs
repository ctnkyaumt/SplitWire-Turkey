namespace SplitWireTurkey.Services;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public sealed class WindowsServiceController
{
    private readonly ICommandRunner _runner;
    private readonly Func<TimeSpan, CancellationToken, Task> _delay;
    private readonly TimeSpan _stopTimeout;
    private readonly TimeSpan _pollInterval;
    private readonly TimeSpan _commandTimeout;

    public WindowsServiceController(ICommandRunner runner,
        Func<TimeSpan, CancellationToken, Task> delay = null,
        TimeSpan? stopTimeout = null,
        TimeSpan? pollInterval = null,
        TimeSpan? commandTimeout = null)
    {
        _runner = runner ?? throw new ArgumentNullException(nameof(runner));
        _delay = delay ?? Task.Delay;
        _stopTimeout = stopTimeout ?? TimeSpan.FromSeconds(15);
        _pollInterval = pollInterval ?? TimeSpan.FromMilliseconds(500);
        _commandTimeout = commandTimeout ?? TimeSpan.FromSeconds(30);
    }

    public async Task<ServiceState> QueryAsync(string serviceName, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
            return ServiceState.Unknown;

        var result = await _runner.RunAsync("sc.exe", $"query {serviceName}", _commandTimeout, ct).ConfigureAwait(false);
        return ServiceStatusParser.Parse(result);
    }

    public async Task<IReadOnlyDictionary<string, ServiceState>> QueryManyAsync(IEnumerable<string> serviceNames, CancellationToken ct = default)
    {
        var dict = new Dictionary<string, ServiceState>(StringComparer.OrdinalIgnoreCase);
        if (serviceNames == null)
            return dict;

        foreach (var name in serviceNames)
        {
            if (string.IsNullOrWhiteSpace(name))
                continue;

            dict[name] = await QueryAsync(name, ct).ConfigureAwait(false);
        }

        return dict;
    }

    public async Task<bool> IsInstalledAsync(string serviceName, CancellationToken ct = default)
    {
        var state = await QueryAsync(serviceName, ct).ConfigureAwait(false);
        return state != ServiceState.NotInstalled && state != ServiceState.Unknown;
    }

    public async Task<ServiceOperationResult> StartAsync(string serviceName, CancellationToken ct = default)
    {
        var result = await _runner.RunAsync("sc.exe", $"start {serviceName}", _commandTimeout, ct).ConfigureAwait(false);
        bool success = result.ExitCode == 0 || result.ExitCode == 1056;
        var finalState = await QueryAsync(serviceName, ct).ConfigureAwait(false);
        var msg = success ? $"{serviceName} başlatıldı." : $"{serviceName} başlatılamadı: {result.StdErr}";
        return new ServiceOperationResult(serviceName, success, finalState, msg);
    }

    public async Task<ServiceOperationResult> StopAsync(string serviceName, IProgress<string> log = null, CancellationToken ct = default)
    {
        log?.Report($"{serviceName} durumu denetleniyor...");
        var state = await QueryAsync(serviceName, ct).ConfigureAwait(false);

        if (state == ServiceState.NotInstalled)
        {
            log?.Report($"{serviceName} kurulu değil.");
            return new ServiceOperationResult(serviceName, true, ServiceState.NotInstalled, $"{serviceName} is not installed.");
        }

        if (state == ServiceState.Stopped)
        {
            log?.Report($"{serviceName} zaten durdurulmuş.");
            return new ServiceOperationResult(serviceName, true, ServiceState.Stopped, $"{serviceName} is already stopped.");
        }

        log?.Report($"{serviceName} hizmeti durduruluyor...");
        await _runner.RunAsync("sc.exe", $"stop {serviceName}", _commandTimeout, ct).ConfigureAwait(false);

        var elapsed = TimeSpan.Zero;
        while (elapsed < _stopTimeout)
        {
            await _delay(_pollInterval, ct).ConfigureAwait(false);
            elapsed += _pollInterval;

            state = await QueryAsync(serviceName, ct).ConfigureAwait(false);
            if (state == ServiceState.Stopped || state == ServiceState.NotInstalled)
            {
                log?.Report($"{serviceName} hizmeti durduruldu.");
                return new ServiceOperationResult(serviceName, true, state, $"{serviceName} stopped successfully.");
            }
        }

        log?.Report($"{serviceName} durdurulamadı (zaman aşımı).");
        return new ServiceOperationResult(serviceName, false, state, $"{serviceName} stop timed out after {_stopTimeout.TotalSeconds}s.");
    }

    public async Task<ServiceOperationResult> StopAndDeleteAsync(string serviceName, IProgress<string> log = null, CancellationToken ct = default)
    {
        var stopResult = await StopAsync(serviceName, log, ct).ConfigureAwait(false);
        if (stopResult.FinalState == ServiceState.NotInstalled)
        {
            return stopResult;
        }

        log?.Report($"{serviceName} hizmeti siliniyor...");
        var deleteResult = await _runner.RunAsync("sc.exe", $"delete {serviceName}", _commandTimeout, ct).ConfigureAwait(false);

        bool deleteSuccess = deleteResult.ExitCode == 0 || deleteResult.ExitCode == 1072 || deleteResult.ExitCode == 1060;
        var finalState = deleteSuccess ? ServiceState.NotInstalled : stopResult.FinalState;
        var msg = deleteSuccess ? $"{serviceName} silindi." : $"{serviceName} silinemedi: {deleteResult.StdErr}";
        return new ServiceOperationResult(serviceName, deleteSuccess, finalState, msg);
    }

    public async Task<IReadOnlyList<ServiceOperationResult>> StopAndDeleteManyAsync(IEnumerable<string> serviceNames, IProgress<string> log = null, CancellationToken ct = default)
    {
        var ordered = ServiceCatalog.OrderForStop(serviceNames);
        var results = new List<ServiceOperationResult>();
        foreach (var name in ordered)
        {
            var res = await StopAndDeleteAsync(name, log, ct).ConfigureAwait(false);
            results.Add(res);
        }
        return results.AsReadOnly();
    }
}
