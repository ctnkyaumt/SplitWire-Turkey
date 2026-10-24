namespace SplitWireTurkey.Services;

using System;
using System.Threading;
using System.Threading.Tasks;

public sealed class ProcessCommandRunner : ICommandRunner
{
    public Task<CommandResult> RunAsync(string fileName, string arguments, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
