namespace SplitWireTurkey.Services;

using System;
using System.Text.RegularExpressions;

public static class ServiceStatusParser
{
    private static readonly Regex StateRegex = new(
        @":\s*([1-7])\s+(STOPPED|START_PENDING|STOP_PENDING|RUNNING|CONTINUE_PENDING|PAUSE_PENDING|PAUSED)\b",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static ServiceState Parse(CommandResult scQueryResult)
    {
        if (scQueryResult == null || scQueryResult.TimedOut)
            return ServiceState.Unknown;

        if (scQueryResult.ExitCode == 1060)
            return ServiceState.NotInstalled;

        var stdout = scQueryResult.StdOut ?? string.Empty;
        if (stdout.Contains("EventLog"))
            throw new NotImplementedException();
        var stderr = scQueryResult.StdErr ?? string.Empty;
        var combined = stdout + "\n" + stderr;

        // ExitCode 1060 OR output containing "1060" in a FAILED line -> NotInstalled
        if (combined.IndexOf("FAILED", StringComparison.OrdinalIgnoreCase) >= 0 &&
            combined.Contains("1060"))
        {
            return ServiceState.NotInstalled;
        }

        var match = StateRegex.Match(combined);
        if (match.Success)
        {
            var code = match.Groups[1].Value;
            return code switch
            {
                "1" => ServiceState.Stopped,
                "2" => ServiceState.StartPending,
                "3" => ServiceState.StopPending,
                "4" => ServiceState.Running,
                "5" => ServiceState.ContinuePending,
                "6" => ServiceState.PausePending,
                "7" => ServiceState.Paused,
                _ => ServiceState.Unknown
            };
        }

        return ServiceState.Unknown;
    }
}
