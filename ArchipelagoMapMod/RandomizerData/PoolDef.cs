﻿namespace ArchipelagoMapMod.RandomizerData;

/// <summary>
/// Data structure representing a collection of items and locations that can be optionally randomized.
/// </summary>
public class PoolDef
{
    /// <summary>
    /// The name of the pool.
    /// </summary>
    public string Name { get; init; }
    /// <summary>
    /// A slightly broader classification which merges smaller pools into larger ones (e.g. Focus into Skill, etc). Used by SplitGroupSettings.
    /// </summary>
    public string Group { get; init; }
    public string Path { get; init; }
    public string[] IncludeItems { get; init; }
    public string[] IncludeLocations { get; init; }
    public VanillaDef[] Vanilla { get; init; }

    public bool IsIncluded(Settings.GenerationSettings gs)
    {
        if (!string.IsNullOrEmpty(Path)
            && Settings.Util.Get(gs, Path) is bool value
            && value) return true;
        else return false;
    }

    public bool IsVanilla(Settings.GenerationSettings gs)
    {
        if (!string.IsNullOrEmpty(Path)
            && Settings.Util.Get(gs, Path) is bool value
            && !value) return true;
        else return false;
    }

}
