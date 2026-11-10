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
        if (results == null)
        {
            return null;
        }

        PresetChoice bestChoice = null;

        foreach (var preset in results)
        {
            if (preset == null || preset.Targets == null || preset.Targets.Count == 0)
            {
                continue;
            }

            bool allSucceeded = true;
            long totalTicks = 0;

            foreach (var target in preset.Targets)
            {
                if (target == null || !target.Success)
                {
                    allSucceeded = false;
                    break;
                }
                totalTicks += target.Elapsed.Ticks;
            }

            if (!allSucceeded)
            {
                continue;
            }

            var totalElapsed = TimeSpan.FromTicks(totalTicks);

            if (bestChoice == null || totalElapsed < bestChoice.TotalElapsed)
            {
                bestChoice = new PresetChoice(preset.PresetName, totalElapsed);
            }
        }

        return bestChoice;
    }
}
