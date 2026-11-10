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
}
