using Archipelago.HollowKnight;
using Archipelago.HollowKnight.IC.RM;

namespace ArchipelagoMapMod.Settings;

public class GenerationSettings
{
    public string StartLocation => ArchipelagoMod.Instance.SlotData.Options.StartLocationName ?? StartLocationNames.Kings_Pass;
    public StartDef StartDef => StartDef.Lookup[StartLocation];

    public TransitionSettings TransitionSettings = new();
    public CostSettings CostSettings = new();
    public CursedSettings CursedSettings = new();
    public SkipSettings SkipSettings = new();
    public MiscSettings MiscSettings = new();
    public PoolSettings PoolSettings = new();
    public NoveltySettings NoveltySettings = new();
}