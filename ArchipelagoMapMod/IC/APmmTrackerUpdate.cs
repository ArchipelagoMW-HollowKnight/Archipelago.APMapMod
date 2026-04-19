using Archipelago.HollowKnight;
using Archipelago.HollowKnight.IC;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using ArchipelagoMapMod.RC;
using ItemChanger;
using Newtonsoft.Json.Linq;

namespace ArchipelagoMapMod.IC;

public partial class APmmTrackerUpdate : ItemChanger.Modules.Module
{
    private const string DATASTORAGE_KEY_VISITED_TRANSITIONS = "visited_transitions";

    private static APmmTrackerUpdate LoadedInstance;
    public static IEnumerable<(string source, string target)> GetKnownVisitedTransitionPairs()
    {
        return LoadedInstance?.GetVisitedTransitionPairs() ?? [];
    }

    public HashSet<string> VisitedTransitions { get; private set; } = [];

    private ArchipelagoSession session;

    [DataStorageProperty(nameof(session), Scope.Slot, DATASTORAGE_KEY_VISITED_TRANSITIONS)]
    private partial DataStorageElement VisitedTransitionsRemote { get; set; }

    public override void Initialize()
    {
        session = ArchipelagoMod.Instance.session;

        ModuleHandlingProperties = ModuleHandlingFlags.AllowDeserializationFailure;
        APmmItemTag.AfterRandoItemGive += AfterRandoItemGive;
        APmmPlacementTag.OnRandoPlacementVisitStateChanged += OnRandoPlacementVisitStateChanged;
        AbstractItem.AfterGiveGlobal += AfterRemoteItemGive;
        Events.OnTransitionOverride += OnTransitionOverride;

        VisitedTransitionsRemote.Initialize(JObject.FromObject(new Dictionary<string, bool>()));
        VisitedTransitionsRemote.OnValueChanged += OnRemoteTransitionsUpdated;
        VisitedTransitionsRemote += Operation.Update(BuildTransitionData(VisitedTransitions));

        try
        {
            Dictionary<string, bool> remoteTransitions = VisitedTransitionsRemote.To<Dictionary<string, bool>>();
            // sync local storage from remote but don't update the map as it is not yet ready.
            MarkTransitionsFromServer(remoteTransitions, false);
        }
        catch (Exception ex)
        {
            ArchipelagoMapMod.Instance.LogError($"Unexpected issue unlocking transitions from server data: {ex}");
        }

        LoadedInstance = this;
    }

    public override void Unload()
    {
        LoadedInstance = null;

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
        string sourceName = source.ToString();
        lock (VisitedTransitions)
        {
            VisitedTransitions.Add(sourceName);
        }
        VisitedTransitionsRemote += Operation.Update(BuildTransitionData([sourceName]));
        OnTransitionFound(sourceName);
    }

    private void OnTransitionFound(string sourceName)
    {
        InitTransitionLookup();
        if (transitionLookup.TryGetValue(sourceName, out string targetName) && !TD.HasVisited(sourceName))
        {
            OnTransitionVisited?.Invoke(sourceName, targetName);

            APRandoContext context = (APRandoContext)ApmmDataModule.Instance.Context;
            if (context.GenerationSettings.TransitionSettings.Coupled && transitionLookup.ContainsKey(targetName))
            {
                OnTransitionVisited?.Invoke(targetName, sourceName);
            }

            try
            {
                OnFinishedUpdate?.Invoke();
            }
            // because this may be invoked from server events it can be before the map is ready, that's ok (probably?)
            catch (Exception ex)
            {
                ArchipelagoMapMod.Instance.LogWarn($"Transition found before map alive, probably harmless?: {ex}");
            }
        }
    }

    private IEnumerable<(string source, string target)> GetVisitedTransitionPairs()
    {
        InitTransitionLookup();
        APRandoContext context = (APRandoContext)ApmmDataModule.Instance.Context;
        lock (VisitedTransitions)
        {
            foreach (string source in VisitedTransitions)
            {
                if (transitionLookup.TryGetValue(source, out string target))
                {
                    yield return (source, target);
                    if (context.GenerationSettings.TransitionSettings.Coupled && transitionLookup.ContainsKey(target))
                    {
                        yield return (target, source);
                    }
                }
            }
        }
    }

    private void OnRemoteTransitionsUpdated(JToken oldData, JToken newData, Dictionary<string, JToken> additionalArguments)
    {
        Dictionary<string, bool> transitions = newData.ToObject<Dictionary<string, bool>>();
        MarkTransitionsFromServer(transitions, true);
    }

    private Dictionary<string, bool> BuildTransitionData(IEnumerable<string> visitedTransitions)
    {
        Dictionary<string, bool> visitedTransitionsDict = [];
        foreach (string t in visitedTransitions)
        {
            visitedTransitionsDict[t] = true;
        }
        return visitedTransitionsDict;
    }

    private void MarkTransitionsFromServer(Dictionary<string, bool> visitedTransitions, bool notifyMap)
    {
        if (visitedTransitions == null)
        {
            return;
        }

        lock (VisitedTransitions)
        {
            VisitedTransitions.UnionWith(visitedTransitions.Keys);
        }

        if (notifyMap == false)
        {
            return;
        }

        foreach (string t in visitedTransitions.Keys)
        {
            OnTransitionFound(t);
        }
    }

    private void InitTransitionLookup()
    {
        transitionLookup ??= TD.ctx.TransitionPlacements.ToDictionary(p => p.Source.Name, p => p.Target.Name);
    }
}

