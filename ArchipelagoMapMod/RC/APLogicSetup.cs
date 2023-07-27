using ArchipelagoMapMod.RandomizerData;
using ArchipelagoMapMod.Settings;
using MapChanger;
using RandomizerCore;

namespace ArchipelagoMapMod.RC;

public class APLogicSetup : HookModule
{
    public override void OnEnterGame()
    {
        Data.Load();
        
        //TODO: fix start location when AP provides the info.
        ArchipelagoMapMod.LS.Context ??= new APRandoContext(new GenerationSettings(), Data.Starts["King's Pass"]);
        ArchipelagoMapMod.LS.TrackerData ??= new TrackerData {AllowSequenceBreaks = true};
        ArchipelagoMapMod.LS.TrackerDataWithoutSequenceBreaks ??= new TrackerData {AllowSequenceBreaks = false};

        ArchipelagoMapMod.LS.Context.Vanilla ??= new List<GeneralizedPlacement>();
        foreach (var transition in Data.Transitions)
        {
            if (transition.Value.VanillaTarget == null) continue;
#if DEBUG
            ArchipelagoMapMod.Instance.LogDebug(
                $"creating transition {transition.Key} and linking it to {transition.Value.VanillaTarget}");
#endif
            var item = ArchipelagoMapMod.LS.Context.LM.TransitionLookup[transition.Key];
            var location = ArchipelagoMapMod.LS.Context.LM.GetTransition(transition.Value.VanillaTarget);
            ArchipelagoMapMod.LS.Context.Vanilla.Add(new GeneralizedPlacement(item, location));
        }
        
        ArchipelagoMapMod.LS.TrackerDataWithoutSequenceBreaks?.Setup(ArchipelagoMapMod.LS.Context);
        ArchipelagoMapMod.LS.TrackerData?.Setup(ArchipelagoMapMod.LS.Context);
    }

    public override void OnQuitToMenu()
    {
    }
}