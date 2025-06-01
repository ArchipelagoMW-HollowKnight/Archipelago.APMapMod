﻿using ArchipelagoMapMod.RandomizerData;

namespace ArchipelagoMapMod.RC;

public static class LogicConstUtil
{
    private static readonly string[] _charmTerms;
    private static readonly Dictionary<string, int> _charmIDs;

    /// <summary>
    /// Returns the term name corresponding to the (1-based) charm ID.
    /// </summary>
    public static string GetCharmTerm(int charmID)
    {
        return _charmTerms[charmID - 1];
    }

    /// <summary>
    /// Returns the (1-based) charm ID corresponding to the charm term name.
    /// </summary>
    public static int GetCharmID(string charmTermName)
    {
        return _charmIDs[charmTermName];
    }

    static LogicConstUtil()
    {
        // TODO: const string fields?
        _charmTerms = JsonUtil.Deserialize<Dictionary<string, List<string>>>($"{ArchipelagoMapMod.MOD}.Resources.Logic.terms.json")["SignedByte"].SkipWhile(t => t != "Gathering_Swarm").Take(40).ToArray();
        _charmIDs = _charmTerms.Select((s, i) => (s, i)).ToDictionary(p => p.s, p => p.i + 1);
        _charmIDs["White_Fragment"] = _charmIDs["Queen_Fragment"] = _charmIDs["King_Fragment"] = _charmIDs["Kingsoul"] = _charmIDs["Void_Heart"] = 36; // aliases of WHITEFRAGMENT
    }
}
