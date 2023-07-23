using System.Collections;
using Benchwarp;
using InControl;
using ItemChanger.Internal;
using Modding;
using UnityEngine;
using JsonUtil = MapChanger.JsonUtil;

namespace ArchipelagoMapMod;

internal record struct apmmBenchKey(string SceneName, string RespawnMarkerName);

internal class BenchwarpInterop
{
    internal const string BENCH_EXTRA_SUFFIX = "_Extra";
    internal const string BENCH_WARP_START = "Start_Warp";

    internal static Dictionary<apmmBenchKey, string> BenchNames { get; private set; } = new();
    internal static Dictionary<string, apmmBenchKey> BenchKeys { get; private set; } = new();
    internal static apmmBenchKey StartKey { get; private set; }

    internal static void Load()
    {
        BenchNames = new Dictionary<apmmBenchKey, string>();
        BenchKeys = new Dictionary<string, apmmBenchKey>();

        if (Interop.HasBenchRando() && BenchRandoInterop.BenchRandoEnabled())
        {
            BenchNames = BenchRandoInterop.GetBenches();
        }
        else
        {
            var benchwarp = JsonUtil.DeserializeFromAssembly<Dictionary<string, string>>(ArchipelagoMapMod.Assembly,
                "ArchipelagoMapMod.Resources.benchwarp.json");

            foreach (var kvp in benchwarp)
            {
                var bench = Bench.Benches.FirstOrDefault(b => b.sceneName == kvp.Key);

                if (bench is null) continue;

                BenchNames.Add(new apmmBenchKey(bench.sceneName, bench.respawnMarker), kvp.Value);
            }
        }

        StartKey = new apmmBenchKey(Ref.Settings.Start.SceneName, "ITEMCHANGER_RESPAWN_MARKER");

        BenchNames.Add(StartKey, BENCH_WARP_START);

        BenchKeys = BenchNames.ToDictionary(t => t.Value, t => t.Key);
    }

    internal static bool IsVisitedBench(string benchName)
    {
        return benchName is BENCH_WARP_START or $"{BENCH_WARP_START}{BENCH_EXTRA_SUFFIX}"
               || ((BenchKeys.TryGetValue(benchName, out var key)
                    || (benchName.Length > BENCH_EXTRA_SUFFIX.Length &&
                        BenchKeys.TryGetValue(benchName.Substring(0, benchName.Length - BENCH_EXTRA_SUFFIX.Length),
                            out key)))
                   && GetVisitedBenchKeys().Contains(key));
    }

    internal static IEnumerable<string> GetVisitedBenchNames()
    {
        return GetVisitedBenchKeys()
            .Where(b => BenchNames.ContainsKey(b))
            .Select(b => BenchNames[b])
            .Concat(new List<string> {BENCH_WARP_START});
    }

    internal static IEnumerator DoBenchwarp(string benchName)
    {
        if (BenchKeys.TryGetValue(benchName, out var benchKey))
            yield return DoBenchwarpInternal(benchKey);
        else if (benchName.Length > BENCH_EXTRA_SUFFIX.Length &&
                 BenchKeys.TryGetValue(benchName.Substring(0, benchName.Length - BENCH_EXTRA_SUFFIX.Length),
                     out benchKey)) yield return DoBenchwarpInternal(benchKey);
    }

    private static IEnumerator DoBenchwarpInternal(apmmBenchKey benchKey)
    {
        InputHandler.Instance.inputActions.openInventory.CommitWithState(true,
            ReflectionHelper.GetField<OneAxisInputControl, ulong>(InputHandler.Instance.inputActions.openInventory,
                "pendingTick") + 1, 0);
        yield return new WaitWhile(() => GameManager.instance.inventoryFSM.ActiveStateName != "Closed");
        yield return new WaitForSeconds(0.15f);
        UIManager.instance.TogglePauseGame();
        yield return new WaitWhile(() => !GameManager.instance.IsGamePaused());
        yield return new WaitForSecondsRealtime(0.1f);
        if (GameManager.instance.IsGamePaused())
        {
            var bench = Bench.Benches.FirstOrDefault(b =>
                b.sceneName == benchKey.SceneName && b.respawnMarker == benchKey.RespawnMarkerName);

            if (bench is not null)
                bench.SetBench();
            else
                Events.SetToStart();

            ChangeScene.WarpToRespawn();
        }
    }

    /// <summary>
    ///     Gets the BenchKeys from Benchwarp's visited benches and converts them to apmmBenchKeys.
    /// </summary>
    private static HashSet<apmmBenchKey> GetVisitedBenchKeys()
    {
        return new HashSet<apmmBenchKey>(Benchwarp.Benchwarp.LS.visitedBenchScenes.Select(bwKey =>
            new apmmBenchKey(bwKey.SceneName, bwKey.RespawnMarkerName)));
    }
}