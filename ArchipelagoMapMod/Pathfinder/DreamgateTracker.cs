using ArchipelagoMapMod.Transition;
using MapChanger;
using On.HutongGames.PlayMaker.Actions;
using Events = ItemChanger.Events;

namespace ArchipelagoMapMod.Pathfinder;

internal class DreamgateTracker : HookModule
{
    internal const string DREAMGATE = "Dreamgate";

    private static bool dreamgateSet;
    private static bool dreamgateUsed;
    internal static string DreamgateScene { get; private set; }
    internal static string DreamgateTiedTransition { get; private set; }

    public override void OnEnterGame()
    {
        dreamgateSet = false;
        dreamgateUsed = false;
        DreamgateScene = PlayerData.instance.dreamGateScene;
        DreamgateTiedTransition = null;

        SetPlayerDataString.OnEnter += TrackDreamgateSet;
        Events.OnBeginSceneTransition += TrackDreamgate;
    }

    public override void OnQuitToMenu()
    {
        SetPlayerDataString.OnEnter -= TrackDreamgateSet;
        Events.OnBeginSceneTransition -= TrackDreamgate;
    }

    private static void TrackDreamgateSet(SetPlayerDataString.orig_OnEnter orig,
        HutongGames.PlayMaker.Actions.SetPlayerDataString self)
    {
        orig(self);

        if (self.stringName.Value is "dreamGateScene")
        {
            dreamgateSet = true;
            DreamgateScene = self.value.Value;
            DreamgateTiedTransition = null;

            //ArchipelagoMapMod.Instance.LogDebug($"Dreamgate set to {DreamgateScene}");
        }
    }

    private static void TrackDreamgate(ItemChanger.Transition lastTransition)
    {
        // If the player left a scene where a dreamgate was just set OR just used, add logic to the transition performed
        if ((dreamgateSet || dreamgateUsed)
            && apmmPathfinder.SD.TransitionTermsByScene.TryGetValue(DreamgateScene, out var transitions))
        {
            //ArchipelagoMapMod.Instance.LogDebug($"Dreamgate was set or used in previous scene. Trying to add logical connection:");

            DreamgateTiedTransition = null;

            // Try getting the last target (excludes benchwarps)
            foreach (var source in transitions.Select(t => t.Name))
                if (TransitionData.Placements.TryGetValue(source, out var target)
                    && target == lastTransition.ToString())
                {
                    DreamgateTiedTransition = source;
                    InstructionData.UpdateDreamgateInstruction(source);
                    //ArchipelagoMapMod.Instance.LogDebug($"Dreamgate tied to {source}");
                    break;
                }
        }

        dreamgateUsed = lastTransition.GateName is "dreamGate";
        dreamgateSet = false;
    }
}