namespace SplitWireTurkey.Tests;

using System;
using SplitWireTurkey.Services;
using Xunit;

public class GoodbyeDpiArgumentsTests
{
    [Fact]
    public void Build_removes_ipv6_options_when_ipv6_not_available()
    {
        const string input = "-5 --set-ttl 5 --dns-addr 1.1.1.1 --dns-port 53 --dnsv6-addr 2606:4700:4700::1111 --dnsv6-port 53";
        const string expected = "-5 --set-ttl 5 --dns-addr 1.1.1.1 --dns-port 53";

        var actual = GoodbyeDpiArguments.Build(input, false, null, false);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Build_retains_ipv6_options_when_ipv6_available()
    {
        const string input = "-5 --set-ttl 5 --dns-addr 1.1.1.1 --dns-port 53 --dnsv6-addr 2606:4700:4700::1111 --dnsv6-port 53";
        const string expected = "-5 --set-ttl 5 --dns-addr 1.1.1.1 --dns-port 53 --dnsv6-addr 2606:4700:4700::1111 --dnsv6-port 53";

        var actual = GoodbyeDpiArguments.Build(input, false, null, true);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Build_appends_quoted_blacklist_when_useSiteList_is_true()
    {
        const string input = "-9";
        const string siteListPath = @"C:\Users\John Doe\AppData\Local\SplitWire-Turkey\GoodbyeDPI\blacklist.txt";
        const string expected = @"-9 --blacklist ""C:\Users\John Doe\AppData\Local\SplitWire-Turkey\GoodbyeDPI\blacklist.txt""";

        var actual = GoodbyeDpiArguments.Build(input, true, siteListPath, false);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Build_replaces_all_existing_blacklists_with_new_blacklist_when_useSiteList_is_true()
    {
        const string input = @"-5 --blacklist old.txt -e 2 --blacklist ""C:\a b\x.txt"" --dns-addr 1.1.1.1";
        const string siteListPath = @"C:\new\list.txt";
        const string expected = @"-5 -e 2 --dns-addr 1.1.1.1 --blacklist ""C:\new\list.txt""";

        var actual = GoodbyeDpiArguments.Build(input, true, siteListPath, true);

        Assert.Equal(expected, actual);
    }
}
