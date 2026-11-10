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
        throw new NotImplementedException();
    }

    public static bool Covers(IEnumerable<string> entries, string host)
    {
        throw new NotImplementedException();
    }
}
