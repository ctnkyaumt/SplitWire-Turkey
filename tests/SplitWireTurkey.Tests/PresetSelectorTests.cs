namespace SplitWireTurkey.Tests;

using System;
using System.Collections.Generic;
using SplitWireTurkey.Services;
using Xunit;

public class PresetSelectorTests
{
    [Fact]
    public void Choose_fastest_all_pass_preset_wins_over_faster_preset_with_failure()
    {
        var presetA = new PresetProbeResult("-5", new[]
        {
            new TargetResult("Target1", true, TimeSpan.FromMilliseconds(50)),
            new TargetResult("Target2", false, TimeSpan.FromMilliseconds(50))
        });

        var presetB = new PresetProbeResult("-9", new[]
        {
            new TargetResult("Target1", true, TimeSpan.FromMilliseconds(100)),
            new TargetResult("Target2", true, TimeSpan.FromMilliseconds(100))
        });

        var choice = PresetSelector.Choose(new[] { presetA, presetB });

        Assert.NotNull(choice);
        Assert.Equal("-9", choice.PresetName);
        Assert.Equal(TimeSpan.FromMilliseconds(200), choice.TotalElapsed);
    }

    [Fact]
    public void Choose_tie_picks_earliest_in_input_order()
    {
        var presetA = new PresetProbeResult("-5", new[]
        {
            new TargetResult("Target1", true, TimeSpan.FromMilliseconds(100))
        });

        var presetB = new PresetProbeResult("-6", new[]
        {
            new TargetResult("Target1", true, TimeSpan.FromMilliseconds(100))
        });

        var choice = PresetSelector.Choose(new[] { presetA, presetB });

        Assert.NotNull(choice);
        Assert.Equal("-5", choice.PresetName);
    }

    [Fact]
    public void Choose_returns_null_when_none_qualify()
    {
        var presetA = new PresetProbeResult("-5", new[]
        {
            new TargetResult("Target1", false, TimeSpan.FromMilliseconds(10))
        });

        Assert.Null(PresetSelector.Choose(new[] { presetA }));
        Assert.Null(PresetSelector.Choose(Array.Empty<PresetProbeResult>()));
        Assert.Null(PresetSelector.Choose(null));
    }

    [Fact]
    public void Choose_preset_with_zero_targets_does_not_qualify()
    {
        var emptyPreset = new PresetProbeResult("-5", Array.Empty<TargetResult>());
        Assert.Null(PresetSelector.Choose(new[] { emptyPreset }));
    }
}
