namespace SplitWireTurkey.Services;

using System;
using System.Collections.Generic;
using System.Text;

public static class GoodbyeDpiArguments
{
    public static string Build(string presetArgs, bool useSiteList, string siteListPath, bool ipv6Available)
    {
        var tokens = Tokenize(presetArgs);
        var retained = new List<string>();

        for (int i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];

            if (!ipv6Available && (string.Equals(token, "--dnsv6-addr", StringComparison.OrdinalIgnoreCase) ||
                                   string.Equals(token, "--dnsv6-port", StringComparison.OrdinalIgnoreCase)))
            {
                if (i + 1 < tokens.Count)
                {
                    i++;
                }
                continue;
            }

            if (useSiteList && string.Equals(token, "--blacklist", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 < tokens.Count)
                {
                    i++;
                }
                continue;
            }

            retained.Add(token);
        }

        if (useSiteList)
        {
            var cleanPath = siteListPath.Trim().Trim('\"');
            retained.Add("--blacklist");
            retained.Add($"\"{cleanPath}\"");
        }

        return string.Join(" ", retained).Trim();
    }

    private static List<string> Tokenize(string input)
    {
        var tokens = new List<string>();
        if (string.IsNullOrWhiteSpace(input))
        {
            return tokens;
        }

        var sb = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            if (c == '"')
            {
                inQuotes = !inQuotes;
                sb.Append(c);
            }
            else if (char.IsWhiteSpace(c) && !inQuotes)
            {
                if (sb.Length > 0)
                {
                    tokens.Add(sb.ToString());
                    sb.Clear();
                }
            }
            else
            {
                sb.Append(c);
            }
        }

        if (sb.Length > 0)
        {
            tokens.Add(sb.ToString());
        }

        return tokens;
    }
}
