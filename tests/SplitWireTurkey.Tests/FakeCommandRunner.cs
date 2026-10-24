namespace SplitWireTurkey.Tests;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SplitWireTurkey.Services;

public sealed class FakeCommandRunner : ICommandRunner
{
    private readonly Dictionary<string, Queue<CommandResult>> _responses = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<string> _callLog = new();

    public IReadOnlyList<string> CallLog => _callLog.AsReadOnly();

    public void EnqueueResponse(string fileName, string arguments, CommandResult result)
    {
        var key = $"{fileName}|{arguments}";
        if (!_responses.TryGetValue(key, out var queue))
        {
            queue = new Queue<CommandResult>();
            _responses[key] = queue;
        }
        queue.Enqueue(result);
    }

    public Task<CommandResult> RunAsync(string fileName, string arguments, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        var key = $"{fileName}|{arguments}";
        _callLog.Add(key);

        if (_responses.TryGetValue(key, out var queue) && queue.Count > 0)
        {
            var res = queue.Count == 1 ? queue.Peek() : queue.Dequeue();
            return Task.FromResult(res);
        }

        return Task.FromResult(new CommandResult(0, string.Empty, string.Empty, false));
    }
}
