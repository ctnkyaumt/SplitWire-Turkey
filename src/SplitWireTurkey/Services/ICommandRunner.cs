namespace SplitWireTurkey.Services;

using System;
using System.Threading;
using System.Threading.Tasks;

public sealed record CommandResult(int ExitCode, string StdOut, string StdErr, bool TimedOut);

public interface ICommandRunner
{
    Task<CommandResult> RunAsync(string fileName, string arguments, TimeSpan timeout, CancellationToken cancellationToken = default);
}
