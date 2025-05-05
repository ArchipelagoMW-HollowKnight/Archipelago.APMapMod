using Archipelago.HollowKnight.IC;
using ArchipelagoMapMod.RC;
using ItemChanger;

namespace ArchipelagoMapMod.IC;

public class APmmTrackerUpdate : ItemChanger.Modules.Module
{
    public override void Initialize()
    {
        ModuleHandlingProperties = ModuleHandlingFlags.AllowDeserializationFailure;
        APmmItemTag.AfterRandoItemGive += AfterRandoItemGive;
        APmmPlacementTag.OnRandoPlacementVisitStateChanged += OnRandoPlacementVisitStateChanged;
        AbstractItem.AfterGiveGlobal += AfterRemoteItemGive;
        Events.OnTransitionOverride += OnTransitionOverride;
        transitionLookup ??= TD.ctx.TransitionPlacements.ToDictionary(p => p.Source.Name, p => p.Target.Name);
    }

    public override void Unload()
    {
        APmmItemTag.AfterRandoItemGive -= AfterRandoItemGive;
        APmmPlacementTag.OnRandoPlacementVisitStateChanged -= OnRandoPlacementVisitStateChanged;
        AbstractItem.AfterGiveGlobal -= AfterRemoteItemGive;
        Events.OnTransitionOverride -= OnTransitionOverride;
        OnUnload?.Invoke();
    }

    public static event Action<string> OnPlacementPreviewed;
    public static event Action<string> OnPlacementCleared;
    public static event Action<int, string, string> OnItemObtained;
    public static event Action<string> OnRemoteItemObtained;
    public static event Action<string, string> OnTransitionVisited;
    public static event Action OnFinishedUpdate;
    public static event Action OnUnload;

    private TrackerData TD => ApmmDataModule.Instance.TrackerData;
    private Dictionary<string, string> transitionLookup;
    
    private void OnRandoPlacementVisitStateChanged(VisitStateChangedEventArgs args)
    {
        if (args.NewFlags.HasFlag(VisitState.Previewed))
        {
            OnPlacementPreviewed?.Invoke(args.Placement.Name);
            OnFinishedUpdate?.Invoke();
        }
    }

    private void AfterRandoItemGive(int id, ReadOnlyGiveEventArgs args)
    {
        string itemName = args.Item.name; // the name of the item that was given (not necessarily the item placed)
        string placementName = args.Placement.Name;

        OnItemObtained?.Invoke(id, itemName, placementName);
        
        if (args.Placement.Items.All(item => item.WasEverObtained()))
        {
            OnPlacementCleared?.Invoke(placementName);
        }

        OnFinishedUpdate?.Invoke();
    }

    private void AfterRemoteItemGive(ReadOnlyGiveEventArgs args)
    {
        if (args.Placement is RemotePlacement)
        {
            OnRemoteItemObtained?.Invoke(args.Orig.name);
            OnFinishedUpdate?.Invoke();
        }
    }

    private void OnTransitionOverride(ItemChanger.Transition source, ItemChanger.Transition origTarget, ITransition newTarget)
    {
        OnTransitionFound(source.ToString());
    }

    private void OnTransitionFound(string sourceName)
    {
        if (transitionLookup.TryGetValue(sourceName, out string targetName) && !TD.HasVisited(sourceName))
        {
            OnTransitionVisited?.Invoke(sourceName, targetName);

            APRandoContext context = (APRandoContext)ApmmDataModule.Instance.Context;
            if (context.GenerationSettings.TransitionSettings.Coupled && transitionLookup.ContainsKey(targetName))
            {
                OnTransitionVisited?.Invoke(targetName, sourceName);
            }

            OnFinishedUpdate?.Invoke();
        }
    }

}

