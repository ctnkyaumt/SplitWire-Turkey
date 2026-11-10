namespace SplitWireTurkey.Tests;

using System;
using System.Collections.Generic;
using SplitWireTurkey.Services;
using Xunit;

public class SiteListMergerTests
{
    [Fact]
    public void Parse_splits_mixed_crlf_lf_and_drops_empty_and_trims()
    {
        const string input = "  discord.com  \r\n\r\n  roblox.com\n\rpornhub.com\r\n   \n";
        var result = SiteListMerger.Parse(input);

        Assert.Equal(new[] { "discord.com", "roblox.com", "pornhub.com" }, result);
    }

    [Fact]
    public void Merge_preserves_user_order_and_custom_entries_and_appends_missing_defaults_case_insensitively()
    {
        var userEntries = new[] { "Custom.org", "Discord.com", "  custom.org  ", "", null, "Another.net" };
        var defaultEntries = new[] { "discord.com", "Roblox.com", "ANOTHER.NET", "Pornhub.com" };

        var merged = SiteListMerger.Merge(userEntries, defaultEntries);

        var expected = new[] { "Custom.org", "Discord.com", "Another.net", "Roblox.com", "Pornhub.com" };
        Assert.Equal(expected, merged);
    }

    [Fact]
    public void Merge_handles_null_inputs()
    {
        Assert.Empty(SiteListMerger.Merge(null, null));
        Assert.Equal(new[] { "a.com" }, SiteListMerger.Merge(new[] { "a.com" }, null));
        Assert.Equal(new[] { "b.com" }, SiteListMerger.Merge(null, new[] { "b.com" }));
    }
}
