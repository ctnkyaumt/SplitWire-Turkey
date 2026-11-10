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
}
