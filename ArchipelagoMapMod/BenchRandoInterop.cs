using BenchRando.IC;
using ItemChanger;
using static BenchRando.BRData;

namespace ArchipelagoMapMod;

internal class BenchRandoInterop
{
    internal static Dictionary<APmmBenchKey, string> GetBenches()
    {
        var bsm = ItemChangerMod.Modules.Get<BRLocalSettingsModule>();
        return bsm.LS.Benches.ToDictionary(
            benchName =>
                new APmmBenchKey(BenchLookup[benchName].SceneName, BenchLookup[benchName].GetRespawnMarkerName()),
            benchName => benchName);
    }

    internal static bool BenchRandoEnabled()
    {
        return ItemChangerMod.Modules.Get<BRLocalSettingsModule>() is BRLocalSettingsModule bsm
               && bsm.LS is not null && bsm.LS.Settings is not null
               && bsm.LS.Settings.IsEnabled();
    }
}