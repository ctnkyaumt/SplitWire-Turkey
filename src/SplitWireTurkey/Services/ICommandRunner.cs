namespace SplitWireTurkey.Services;

public sealed record CommandResult(int ExitCode, string StdOut, string StdErr, bool TimedOut);

public interface ICommandRunner
{
    Task<CommandResult> RunAsync(string fileName, string arguments, TimeSpan timeout, CancellationToken cancellationToken = default);
}
