namespace SplitWireTurkey.Services;

using System;
using System.Collections.Generic;

public static class SiteListMerger
{
    public static IReadOnlyList<string> Parse(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return Array.Empty<string>();
        }

        var lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        var list = new List<string>();

        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (!string.IsNullOrEmpty(trimmed))
            {
                list.Add(trimmed);
            }
        }

        return list;
    }

    public static IReadOnlyList<string> Merge(IEnumerable<string> userEntries, IEnumerable<string> defaultEntries)
    {
        var result = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (userEntries != null)
        {
            foreach (var entry in userEntries)
            {
                if (entry == null)
                {
                    continue;
                }
                var trimmed = entry.Trim();
                if (trimmed.Length > 0 && seen.Add(trimmed))
                {
                    result.Add(trimmed);
                }
            }
        }

        if (defaultEntries != null)
        {
            foreach (var entry in defaultEntries)
            {
                if (entry == null)
                {
                    continue;
                }
                var trimmed = entry.Trim();
                if (trimmed.Length > 0 && seen.Add(trimmed))
                {
                    result.Add(trimmed);
                }
            }
        }

        return result;
    }

    public static bool Covers(IEnumerable<string> entries, string host)
    {
        if (entries == null || string.IsNullOrWhiteSpace(host))
        {
            return false;
        }

        var trimmedHost = host.Trim();
        if (trimmedHost.Length == 0)
        {
            return false;
        }

        foreach (var rawEntry in entries)
        {
            if (rawEntry == null)
            {
                continue;
            }

            var entry = rawEntry.Trim();
            if (entry.Length == 0 || entry.StartsWith("#", StringComparison.Ordinal))
            {
                continue;
            }

            if (string.Equals(trimmedHost, entry, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (trimmedHost.EndsWith("." + entry, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
