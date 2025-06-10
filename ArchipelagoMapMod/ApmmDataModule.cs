using Archipelago.HollowKnight;
using ArchipelagoMapMod.IC;
using ArchipelagoMapMod.RandomizerData;
using ArchipelagoMapMod.RC;
using ArchipelagoMapMod.RC.StateVariables;
using ArchipelagoMapMod.Settings;
using ArchipelagoMapMod.UI.SkipOptionsGrid;
using ItemChanger;
using MapChanger.UI;
using RandoMapCore;
using RandoMapCore.Data;
using RandomizerCore;
using RandomizerCore.Logic;
using RandomizerCore.Logic.StateLogic;
using System.Text.RegularExpressions;

namespace ArchipelagoMapMod;
public class ApmmDataModule : RmcDataModule
{
    public static ApmmDataModule Instance { get; } = new();
    private ApmmDataModule() { }

    public TrackerData TrackerData { get; private set; }
    public TrackerData TrackerDataNoSequenceBreaks { get; private set; }

    public override string ModName => ArchipelagoMapMod.MOD;

    public override bool IsCorrectSaveType => ArchipelagoMapMod.Instance.IsInApSave;

    public override bool EnableSpoilerToggle => !ArchipelagoMod.Instance.SlotData.DisableLocalSpoilerLogs;
    public override bool EnableProgressionHints => false;

    private Dictionary<string, RmcTransitionDef> randomizedTransitions;
    public override IReadOnlyDictionary<string, RmcTransitionDef> RandomizedTransitions => randomizedTransitions;

    private Dictionary<string, RmcTransitionDef> vanillaTransitions;
    public override IReadOnlyDictionary<string, RmcTransitionDef> VanillaTransitions => vanillaTransitions;

    private Dictionary<RmcTransitionDef, RmcTransitionDef> randomizedTransitionPlacements;
    public override IReadOnlyDictionary<RmcTransitionDef, RmcTransitionDef> RandomizedTransitionPlacements => randomizedTransitionPlacements;

    private Dictionary<RmcTransitionDef, RmcTransitionDef> vanillaTransitionPlacements;
    public override IReadOnlyDictionary<RmcTransitionDef, RmcTransitionDef> VanillaTransitionPlacements => vanillaTransitionPlacements;

    public override bool IsCoupledRando => true;

    private Dictionary<string, RmcLocationDef> randomizedLocations;
    public override IReadOnlyDictionary<string, RmcLocationDef> RandomizedLocations => randomizedLocations;

    private Dictionary<string, RmcLocationDef> vanillaLocations;
    public override IReadOnlyDictionary<string, RmcLocationDef> VanillaLocations => vanillaLocations;

    public override ProgressionManager PM => TrackerData.pm;
    public override ProgressionManager PMNoSequenceBreak => TrackerDataNoSequenceBreaks.pm;
    public override Term StartTerm => ((ProgressionInitializer)Context.InitialProgression).StartStateTerm;
    public override IReadOnlyCollection<Term> StartStateLinkedTerms => ((ProgressionInitializer)Context.InitialProgression).StartStateLinkedTerms;
    public override StateModifier WarpToBenchReset => (WarpToBenchResetVariable)PM.lm.GetVariableStrict(WarpToBenchResetVariable.Prefix);
    public override StateModifier WarpToStartReset => (WarpToStartResetVariable)PM.lm.GetVariable(WarpToStartResetVariable.Prefix);

    public override IReadOnlyCollection<string> UncheckedReachableTransitions => TrackerData.uncheckedReachableTransitions;
    public override IReadOnlyCollection<string> UncheckedReachableTransitionsNoSequenceBreak => TrackerDataNoSequenceBreaks.uncheckedReachableTransitions;
    public override IReadOnlyDictionary<string, string> VisitedTransitions => TrackerData.visitedTransitions;
    public override IReadOnlyCollection<string> OutOfLogicVisitedTransitions => TrackerDataNoSequenceBreaks.outOfLogicVisitedTransitions;

    private APRandoContext context;
    public override RandoContext Context => context;
    public override IEnumerable<RandoPlacement> RandomizedPlacements => context.ItemPlacements
        .Select(ip => new RandoPlacement(ip.Item, ip.Location))
        .Concat(context.TransitionPlacements.Select(tp => new RandoPlacement(tp.Source, tp.Target)));
    public override IEnumerable<GeneralizedPlacement> VanillaPlacements => context.Vanilla;

    public override IReadOnlyDictionary<RmcBenchKey, string> GetCustomBenches()
    {
        if (Interop.HasBenchRando() && BenchRandoInterop.BenchRandoEnabled())
        {
            return BenchRandoInterop.GetBenches();
        }
        return null;
    }

    public override RandoPlacement GetItemRandoPlacement(AbstractItem item)
    {
        if (item.GetTag(out APmmItemTag tag))
        {
            ItemPlacement p = context.ItemPlacements[tag.id];
            return new RandoPlacement(p.Item, p.Location);
        }
        return default;
    }

    public override string GetMapArea(string scene)
    {
        return Data.GetRoomDef(scene)?.MapArea;
    }

    public override void OnEnterGame()
    {
        Data.Load();

        context = new APRandoContext(new GenerationSettings());
        ResetTracker();

        randomizedTransitions = [];
        vanillaTransitions = [];
        randomizedTransitionPlacements = [];
        vanillaTransitionPlacements = [];
        randomizedLocations = [];
        vanillaLocations = [];

        foreach (TransitionPlacement tp in context.TransitionPlacements)
        {
            RmcTransitionDef source = ConvertTransitionDef(tp.Source.TransitionDef);
            RmcTransitionDef target = ConvertTransitionDef(tp.Target.TransitionDef);

            randomizedTransitions[tp.Source.Name] = source;
            randomizedTransitions[tp.Target.Name] = target;
            randomizedTransitionPlacements[source] = target;
        }

        foreach (ItemPlacement ip in context.ItemPlacements)
        {
            if (ip.Location.LocationDef is null)
            {
                ArchipelagoMapMod.Instance.LogFine($"No location def for {ip.Location.Name}");
            }
            randomizedLocations[ip.Location.Name] = ConvertLocationDef(ip.Location.LocationDef);
        }

        foreach (GeneralizedPlacement vp in context.Vanilla)
        {
            if (TryGetTransitionDef(vp.Location.Name, out RmcTransitionDef sourceTransition)
                && TryGetTransitionDef(vp.Item.Name, out RmcTransitionDef targetTransition))
            {
                vanillaTransitions[vp.Location.Name] = sourceTransition;
                vanillaTransitions[vp.Item.Name] = targetTransition;
                vanillaTransitionPlacements[sourceTransition] = targetTransition;
            }
            else if (Data.GetLocationDef(vp.Location.Name) is LocationDef ld)
            {
                vanillaLocations[vp.Location.Name] = ConvertLocationDef(ld);
            }
            else
            {
                ArchipelagoMapMod.Instance.LogWarn("Found vanilla placement which was not a transition or location:"
                    + $"{vp.Item.Name} at {vp.Location.Name}");
            }
        }

        ItemChangerMod.Modules.GetOrAdd<APmmTrackerUpdate>();
        APmmTrackerUpdate.OnFinishedUpdate += PlacementTracker.OnUpdate;
        HintTracker.OnArchipelagoHintUpdate += PlacementTracker.OnUpdate;
    }

    public override void OnQuitToMenu()
    {
        APmmTrackerUpdate.OnFinishedUpdate -= PlacementTracker.OnUpdate;
        HintTracker.OnArchipelagoHintUpdate -= PlacementTracker.OnUpdate;

        TrackerData.UnhookTrackerUpdate();
        TrackerData = null;
        TrackerDataNoSequenceBreaks.UnhookTrackerUpdate();
        TrackerDataNoSequenceBreaks = null;
        randomizedTransitions = null;
        vanillaTransitions = null;
        randomizedTransitionPlacements = null;
        randomizedLocations = null;
        vanillaLocations = null;
    }

    public override IEnumerable<MainButton> GetPauseMenuMainButtons()
    {
        return [new SkipOptionsControlButton()];
    }

    public override IEnumerable<ExtraButtonGrid> GetPauseMenuExtraButtonGrids()
    {
        return [new SkipOptionsGrid()];
    }

    public void ResetTracker()
    {
        TrackerData?.UnhookTrackerUpdate();
        TrackerData = new TrackerData(true);
        TrackerData.Setup(context);

        TrackerDataNoSequenceBreaks?.UnhookTrackerUpdate();
        TrackerDataNoSequenceBreaks = new TrackerData(false);
        TrackerDataNoSequenceBreaks.Setup(context);
    }

    private bool TryGetTransitionDef(string name, out RmcTransitionDef transitionDef)
    {
        if (Data.GetTransitionDef(name) is TransitionDef td)
        {
            transitionDef = ConvertTransitionDef(td);
            return true;
        }

        Match sourceMatch = Regex.Match(name, @"^(\w+)\[(\w+)\]$");
        if (sourceMatch.Success && Data.IsRoom(sourceMatch.Groups[1].Value))
        {
            transitionDef = new RmcTransitionDef
            {
                SceneName = sourceMatch.Groups[1].Value,
                DoorName = sourceMatch.Groups[2].Value,
            };
            return true;
        }

        transitionDef = null;
        return false;
    }

    private RmcTransitionDef ConvertTransitionDef(TransitionDef def)
    {
        return new RmcTransitionDef
        {
            SceneName = def.SceneName,
            DoorName = def.DoorName,
            VanillaTarget = def.VanillaTarget,
        };
    }


    private RmcLocationDef ConvertLocationDef(LocationDef def)
    {
        if (def == null)
        {
            return null;
        }
        return new RmcLocationDef
        {
            Name = def.Name,
            SceneName = def.SceneName,
        };
    }
}
