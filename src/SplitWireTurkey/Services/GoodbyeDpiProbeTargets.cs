namespace SplitWireTurkey.Services;

using System;
using System.Collections.Generic;

public enum ProbeKind
{
    Government,
    Blocked,
    Normal
}

public sealed record ProbeTarget(string Name, string Url, string Host, ProbeKind Kind);

public static class GoodbyeDpiProbeTargets
{
    public static IReadOnlyList<ProbeTarget> Default { get; } = new List<ProbeTarget>
    {
        // Government
        new("MHRS", "https://mhrs.gov.tr/", "mhrs.gov.tr", ProbeKind.Government),
        new("e-Nabiz", "https://enabiz.gov.tr/", "enabiz.gov.tr", ProbeKind.Government),
        new("e-Devlet", "https://giris.turkiye.gov.tr/", "giris.turkiye.gov.tr", ProbeKind.Government),

        // Blocked
        new("Discord", "https://discord.com/", "discord.com", ProbeKind.Blocked),
        new("Roblox", "https://www.roblox.com/", "www.roblox.com", ProbeKind.Blocked),
        new("Pornhub", "https://www.pornhub.com/", "www.pornhub.com", ProbeKind.Blocked),
        new("Pastebin", "https://pastebin.com/", "pastebin.com", ProbeKind.Blocked),

        // Normal
        new("Google", "https://www.google.com/", "www.google.com", ProbeKind.Normal)
    }.AsReadOnly();
}
