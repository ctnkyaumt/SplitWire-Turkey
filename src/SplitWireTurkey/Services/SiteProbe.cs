namespace SplitWireTurkey.Services;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public sealed class SiteProbe
{
    private readonly Func<HttpMessageHandler> _handlerFactory;

    public SiteProbe(Func<HttpMessageHandler> handlerFactory = null)
    {
        _handlerFactory = handlerFactory;
    }

    public async Task<IReadOnlyList<TargetResult>> ProbeAsync(
        IEnumerable<ProbeTarget> targets,
        TimeSpan timeout,
        CancellationToken ct = default)
    {
        if (targets == null)
        {
            return Array.Empty<TargetResult>();
        }

        var targetList = targets.ToList();
        if (targetList.Count == 0)
        {
            return Array.Empty<TargetResult>();
        }

        HttpMessageHandler handler = _handlerFactory != null
            ? _handlerFactory()
            : new SocketsHttpHandler
            {
                UseProxy = false,
                AllowAutoRedirect = false,
                ConnectTimeout = timeout
            };

        using (handler)
        using (var client = new HttpClient(handler, disposeHandler: false))
        {
            var tasks = targetList.Select(target => ProbeSingleAsync(client, target, timeout, ct));
            var results = await Task.WhenAll(tasks).ConfigureAwait(false);
            return results;
        }
    }

    private static async Task<TargetResult> ProbeSingleAsync(
        HttpClient client,
        ProbeTarget target,
        TimeSpan timeout,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(timeout);

        var sw = Stopwatch.StartNew();
        bool success = false;

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, target.Url);
            using var response = await client.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cts.Token).ConfigureAwait(false);

            success = true;
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            success = false;
        }
        catch (Exception)
        {
            ct.ThrowIfCancellationRequested();
            success = false;
        }
        finally
        {
            sw.Stop();
        }

        return new TargetResult(target.Name, success, sw.Elapsed);
    }
}
