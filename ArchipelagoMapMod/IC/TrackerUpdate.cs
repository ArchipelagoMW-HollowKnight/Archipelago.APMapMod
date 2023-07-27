using ArchipelagoMapMod.RC;
using ItemChanger;

namespace ArchipelagoMapMod.IC;

public class TrackerUpdate : ItemChanger.Modules.Module
{
    public override void Initialize()
    {
        APmmItemTag.AfterRandoItemGive += AfterRandoItemGive;
        APmmPlacementTag.OnRandoPlacementVisitStateChanged += OnRandoPlacementVisitStateChanged;
        Events.OnTransitionOverride += OnTransitionOverride;
        transitionLookup ??= TD.ctx.transitionPlacements.ToDictionary(p => p.Source.Name, p => p.Target.Name);
    }

    public override void Unload()
    {
        APmmItemTag.AfterRandoItemGive -= AfterRandoItemGive;
        APmmPlacementTag.OnRandoPlacementVisitStateChanged -= OnRandoPlacementVisitStateChanged;
        Events.OnTransitionOverride -= OnTransitionOverride;
        OnUnload?.Invoke();
    }

    public static event Action<string> OnPlacementPreviewed;
    public static event Action<string> OnPlacementCleared;
    public static event Action<int, string, string> OnItemObtained;
    public static event Action<string, string> OnTransitionVisited;
    public static event Action OnFinishedUpdate;
    public static event Action OnFoundTransitionsCleared;
    public static event Action OnPreviewsCleared;
    public static event Action OnUnload;

    private TrackerData TD => ArchipelagoMapMod.LS.TrackerData;
    private TrackerData TD_WSB => ArchipelagoMapMod.LS.TrackerDataWithoutSequenceBreaks;
    private Dictionary<string, string> transitionLookup;

    private void OnRandoPlacementVisitStateChanged(VisitStateChangedEventArgs args)
    {
        if ((args.NewFlags & VisitState.Previewed) == VisitState.Previewed)
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
        
        if (args.Placement.GetTag<APmmPlacementTag>() is APmmPlacementTag apmmpt && apmmpt.ids.All(i => APmmTracker.Items[i].WasEverObtained()))
        {
            OnPlacementCleared?.Invoke(placementName);
        }

        OnFinishedUpdate?.Invoke();
    }

    /// <summary>
    /// Static method intended to allow updating visited source transitions by external callers.
    /// </summary>
    public static void SendTransitionFound(ItemChanger.Transition source)
    {
        if (ItemChangerMod.Modules.Get<TrackerUpdate>() is TrackerUpdate instance) instance.OnTransitionFound(source.ToString());
    }

    public static void ClearFoundTransitions()
    {
        OnFoundTransitionsCleared?.Invoke();
        OnFinishedUpdate?.Invoke();
    }

    public static void ClearPreviewedPlacements() 
    {
        OnPreviewsCleared?.Invoke();
        OnFinishedUpdate?.Invoke();
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
            
            // TODO: Revisit once transitional randomization is enabled in AP and this info becomes available.
            // if (global::ArchipelagoMapMod.LS.GenerationSettings.TransitionSettings.Coupled && transitionLookup.ContainsKey(targetName))
            // {
            //     OnTransitionVisited?.Invoke(targetName, sourceName);
            // }

            OnFinishedUpdate?.Invoke();
        }
    }

}

