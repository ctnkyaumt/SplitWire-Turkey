namespace SplitWireTurkey.Services;

using System;
using System.Collections.Generic;

public sealed record TargetResult(string Target, bool Success, TimeSpan Elapsed);
public sealed record PresetProbeResult(string PresetName, IReadOnlyList<TargetResult> Targets);
public sealed record PresetChoice(string PresetName, TimeSpan TotalElapsed);

public static class PresetSelector
{
    public static PresetChoice Choose(IEnumerable<PresetProbeResult> results)
    {
        throw new NotImplementedException();
    }
}
