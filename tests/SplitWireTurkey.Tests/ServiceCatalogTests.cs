namespace SplitWireTurkey.Tests;

using System;
using System.Collections.Generic;
using SplitWireTurkey.Services;
using Xunit;

public class ServiceCatalogTests
{
    [Fact]
    public void OrderForStop_sorts_by_catalog_order_canonicalizes_casing_deduplicates_and_places_unknowns_last()
    {
        var input = new[] { "byedpi", "GoodbyeDPI", "custom-service", "zapret", "GOODBYEDPI", "another-unknown" };
        var expected = new[] { "zapret", "GoodbyeDPI", "ByeDPI", "custom-service", "another-unknown" };

        var result = ServiceCatalog.OrderForStop(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void StopOrder_matches_exact_specification_and_order()
    {
        var expected = new[]
        {
            "zapret",
            "GoodbyeDPI",
            "WinDivert",
            "winws1",
            "winws2",
            "wiresock-client-service",
            "ProxiFyreService",
            "ByeDPI"
        };

        Assert.Equal(expected, ServiceCatalog.StopOrder);
        Assert.Equal(expected, ServiceCatalog.AllKnown);
    }

    [Fact]
    public void CurrentServices_and_LegacyServices_contain_expected_service_names()
    {
        var expectedCurrent = new[] { "GoodbyeDPI", "WinDivert", "ByeDPI", "ProxiFyreService" };
        var expectedLegacy = new[] { "zapret", "winws1", "winws2", "wiresock-client-service" };

        Assert.Equal(expectedCurrent, ServiceCatalog.CurrentServices);
        Assert.Equal(expectedLegacy, ServiceCatalog.LegacyServices);
    }

    [Fact]
    public void OrderForStop_handles_null_and_empty()
    {
        Assert.Empty(ServiceCatalog.OrderForStop(null));
        Assert.Empty(ServiceCatalog.OrderForStop(Array.Empty<string>()));
    }
}
