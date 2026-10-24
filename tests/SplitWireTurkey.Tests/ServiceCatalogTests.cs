namespace SplitWireTurkey.Tests;

using SplitWireTurkey.Services;
using Xunit;

public class ServiceCatalogTests
{
    [Fact]
    public void OrderForStop_sorts_by_catalog_order_canonicalizes_casing_deduplicates_and_places_unknowns_last()
    {
        var input = new[] { ""byedpi"", ""GoodbyeDPI"", ""custom-service"", ""zapret"", ""GOODBYEDPI"", ""another-unknown"" };
        var expected = new[] { ""zapret"", ""GoodbyeDPI"", ""ByeDPI"", ""custom-service"", ""another-unknown"" };

        var result = ServiceCatalog.OrderForStop(input);

        Assert.Equal(expected, result);
    }
}
