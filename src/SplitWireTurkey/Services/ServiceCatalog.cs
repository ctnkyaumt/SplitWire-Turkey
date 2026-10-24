namespace SplitWireTurkey.Services;

using System;
using System.Collections.Generic;
using System.Linq;

public static class ServiceCatalog
{
    public static IReadOnlyList<string> StopOrder { get; } = new[]
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

    public static IReadOnlyList<string> CurrentServices { get; } = new[]
    {
        "GoodbyeDPI",
        "WinDivert",
        "ByeDPI",
        "ProxiFyreService"
    };

    public static IReadOnlyList<string> LegacyServices { get; } = new[]
    {
        "zapret",
        "winws1",
        "winws2",
        "wiresock-client-service"
    };

    public static IReadOnlyList<string> AllKnown => StopOrder;

    public static IReadOnlyList<string> OrderForStop(IEnumerable<string> names)
    {
        if (names == null)
            return Array.Empty<string>();

        var knownMap = new Dictionary<string, (int OrderIndex, string CanonicalName)>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < StopOrder.Count; i++)
        {
            knownMap[StopOrder[i]] = (i, StopOrder[i]);
        }

        var knownFound = new Dictionary<string, (int OrderIndex, string CanonicalName)>(StringComparer.OrdinalIgnoreCase);
        var unknownFound = new List<string>();
        var seenUnknowns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var name in names)
        {
            if (string.IsNullOrWhiteSpace(name))
                continue;

            if (knownMap.TryGetValue(name, out var info))
            {
                if (!knownFound.ContainsKey(name))
                {
                    knownFound[name] = info;
                }
            }
            else
            {
                if (seenUnknowns.Add(name))
                {
                    unknownFound.Add(name);
                }
            }
        }

        var result = knownFound.Values
            .OrderBy(k => k.OrderIndex)
            .Select(k => k.CanonicalName)
            .Concat(unknownFound)
            .ToList();

        return result.AsReadOnly();
    }
}
