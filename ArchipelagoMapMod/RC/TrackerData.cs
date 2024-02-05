using Archipelago.HollowKnight.IC;
using ArchipelagoMapMod.IC;
using ArchipelagoMapMod.Pins;
using ItemChanger;
using ItemChanger.Internal;
using Newtonsoft.Json;
using RandomizerCore;
using RandomizerCore.Logic;
using RandomizerCore.Logic.StateLogic;
using RandomizerCore.Updater;

namespace ArchipelagoMapMod.RC;

// class for storing data related to seed progress
// updating handled through IC.TrackerUpdate
// Note: tracking may fail if placement names do not match the corresponding RandoLocation names in the RawSpoiler.
// tracking does not depend on item names
public class TrackerData
{

    /// <summary>
    /// The CTX indices of the items that have been obtained.
    /// </summary>
    [JsonIgnore]
    public HashSet<int> obtainedItems = new();
    /// <summary>
    /// A set which tracks the placements which have been previewed, by the Name property of the corresponding RandoLocation.
    /// </summary>
    public HashSet<string> previewedLocations = new();
    /// <summary>
    /// A dictionary which tracks the transitions that have been visited. Keys are sources and values are their targets.
    /// </summary>
    public Dictionary<string, string> visitedTransitions = new();
    /// <summary>
    /// A set which tracks the placements which have all items obtained, by the Name property of the corresponding RandoLocation.
    /// </summary>
    public HashSet<string> clearedLocations = new();
    /// <summary>
    /// A set which tracks the placements which are reachable in logic and have items remaining and have not been previewed, by the Name property of the corresponding RandoLocation.
    /// </summary>
    public HashSet<string> uncheckedReachableLocations = new();
    /// <summary>
    /// A set which tracks the transitions which are reachable in logic and have not been visited.
    /// </summary>
    public HashSet<string> uncheckedReachableTransitions = new();
    
    /// <summary>
    /// The ProgressionManager for the current state, with the information available to the player.
    /// </summary>
    [JsonIgnore] public ProgressionManager pm;
    [JsonIgnore] public LogicManager lm;
    [JsonIgnore] public APRandoContext ctx;
    private MainUpdater mu;
    
    public void Setup(APRandoContext ctx)
    {
        this.ctx = ctx;
        lm = ctx.LM;

        Reset();
        HookTrackerUpdate();
    }

    public void Reset()
    {
        ArchipelagoMapMod.Instance.Log("Setting up TrackerData...");
        pm = new ProgressionManager(lm, ctx);

        mu = pm.mu;

        // note: location costs are ignored in the tracking, to prevent providing unintended information, by using p.location.logic rather than p.location
        // it is assumed that no information is divulged from the regular location logic and transition logic

        pm.AfterAddItem += (i) =>
        {
            if (i is LogicWaypoint w && w.term.Type != TermType.State)
            {
                AppendWaypointToDebug(w);
            }
            if (i is StateTransmittingUpdateEntry.StateSetter st)
            {
                AppendTransmittedStateToDebug(st.term, st.value);
            }
            if (i is StateUpdateEntry.StateSetter su)
            {
                AppendImprovedStateToDebug(su.term, su.value);
            }
        };
        
        AppendToDebug("Adding waypoints");
        mu.AddWaypoints(lm.Waypoints);
        AppendToDebug("Adding transitions");
        mu.AddTransitions(lm.TransitionLookup.Values);
        AppendToDebug("Adding Vanilla entries");
        mu.AddEntries(ctx.Vanilla.Select(v => new DelegateUpdateEntry(v.Location, pm =>
        {
            AppendVanillaToDebug(v);
            pm.Add(v.Item, v.Location);
            if (v.Location is ILocationWaypoint ilw)
            {
                pm.Add(ilw.GetReachableEffect());
            }
        })));
        AppendToDebug("Adding randomized entries");
        mu.AddEntries(ctx.ItemPlacements.Select((p, id) => new DelegateUpdateEntry(p.Location.logic, OnCanGetLocation(id))));
        AppendToDebug("Adding transitions");
        mu.AddEntries(ctx.TransitionPlacements.Select((p, id) => new DelegateUpdateEntry(p.Source, OnCanGetTransition(id))));
        AppendToDebug("running first update cycle");
        mu.StartUpdating(); // automatically handle tracking reachable unobtained locations/transitions and adding vanilla progression to pm

        AppendToDebug($"Finished logic setup, marking previously obtained items.");
        
        foreach (AbstractPlacement placement in Ref.Settings.Placements.Values)
        {
            if (placement.AllObtained())
            {
                clearedLocations.Add(placement.Name);
                uncheckedReachableLocations.Remove(placement.Name);
            }

            if (placement.Visited.HasFlag(VisitState.Previewed))
            {
                previewedLocations.Add(placement.Name);
            }

            foreach (AbstractItem placementItem in placement.Items)
            {
                if (!placementItem.GetTag(out APmmItemTag tag))
                {
                    continue;
                }

                if (!placementItem.WasEverObtained())
                {
                    continue;
                }

                ItemPlacement ctxItem = ctx.ItemPlacements[tag.id];
                AppendToDebug($"[Tracker-Data] Adding previously obtained item {ctxItem.Item.Name} to Obtained Items under index {tag.id}");
                obtainedItems.Add(ctxItem.Index);
            }
        }
        
        // AP puts all remote item in a placement called `Remote_Items`
        if (Ref.Settings.Placements.TryGetValue(RemotePlacement.SINGLETON_NAME, out AbstractPlacement pmt))
        {
            foreach (AbstractItem remoteItem in pmt.Items)
            {
                AddRemoteItem(remoteItem.name);
            }
        }
        
        foreach (int i in obtainedItems)
        {
            if (i == -1 || ctx.ItemPlacements.Count < i-1)
            {
                ArchipelagoMapMod.Instance.LogError($"invalid Index {i} found in obtained items");
                ArchipelagoMapMod.Instance.LogError($"obtained Items: {string.Join(", ", obtainedItems)}");
                continue;
            }

            (RandoItem item, RandoLocation loc) = ctx.ItemPlacements[i];
            AppendRandoItemToDebug(item, loc);
            pm.Add(item, loc);
        }

        foreach (KeyValuePair<string, string> kvp in visitedTransitions)
        {

            LogicTransition tt = lm.GetTransitionStrict(kvp.Value);
            LogicTransition st = lm.GetTransitionStrict(kvp.Key);

            if (!pm.Has(st.term))
            {
                AppendProgressionTransitionToDebug(st);
                pm.Add(st.GetReachableEffect());
            }

            AppendTransitionTargetToDebug(tt, st);
            pm.Add(tt, st);
        }
    }

    private ItemPlacement AddRemoteItem(string itemName)
    {
        APItem item = new(lm.GetItem(itemName));
        APLocation location = new(lm.GetLogicDef("Remote"));
        ItemPlacement itemPlacement = new(item, location)
        {
            Index = ctx.ItemPlacements.Count
        };
        ctx.ItemPlacements.Add(itemPlacement);
        AppendToDebug($"[Tracker-Data] Adding Remote item {itemName} to Remote Placement under index {itemPlacement.Index}");
        obtainedItems.Add(itemPlacement.Index);
        return itemPlacement;
    }

    private void HookTrackerUpdate()
    {
        APmmTrackerUpdate.OnItemObtained += OnItemObtained;
        APmmTrackerUpdate.OnPlacementPreviewed += OnPlacementPreviewed;
        APmmTrackerUpdate.OnPlacementCleared += OnPlacementCleared;
        APmmTrackerUpdate.OnTransitionVisited += OnTransitionVisited;
        APmmTrackerUpdate.OnPreviewsCleared += OnPreviewsCleared;
        APmmTrackerUpdate.OnFoundTransitionsCleared += OnFoundTransitionsCleared;
        APmmTrackerUpdate.OnUnload += UnhookTrackerUpdate;
        AbstractItem.AfterGiveGlobal += OnAfterGiveGlobal;
    }

    public void UnhookTrackerUpdate()
    {
        APmmTrackerUpdate.OnItemObtained -= OnItemObtained;
        APmmTrackerUpdate.OnPlacementPreviewed -= OnPlacementPreviewed;
        APmmTrackerUpdate.OnPlacementCleared -= OnPlacementCleared;
        APmmTrackerUpdate.OnTransitionVisited -= OnTransitionVisited;
        APmmTrackerUpdate.OnPreviewsCleared -= OnPreviewsCleared;
        APmmTrackerUpdate.OnFoundTransitionsCleared -= OnFoundTransitionsCleared;
        APmmTrackerUpdate.OnUnload -= UnhookTrackerUpdate;
        AbstractItem.AfterGiveGlobal -= OnAfterGiveGlobal;
    }
    
    private void OnAfterGiveGlobal(ReadOnlyGiveEventArgs args)
    {
        if (args.Placement is not RemotePlacement)
        {
            return;
        }

        ItemPlacement itemPlacement = AddRemoteItem(args.Item.name);
        pm.Add(itemPlacement.Item, itemPlacement.Location);
        APmmPinManager.UpdateRandoPins();
    }

    private Action<ProgressionManager> OnCanGetLocation(int id)
    {
        return pm =>
        {
            (RandoItem item, RandoLocation location) = ctx.ItemPlacements[id];
            AppendRandoLocationToDebug(location);
            if (location is ILocationWaypoint ilw)
            {
                pm.Add(ilw.GetReachableEffect());
            }
            if (!clearedLocations.Contains(location.Name) && !previewedLocations.Contains(location.Name))
            {
                uncheckedReachableLocations.Add(location.Name);
            }
        };
    }

    private Action<ProgressionManager> OnCanGetTransition(int id)
    {
        return pm =>
        {
            (RandoTransition target, RandoTransition source) = ctx.TransitionPlacements[id];
            AppendReachableTransitionToDebug(source.lt);
            
            if (!pm.Has(source.lt.term))
            {
                AppendProgressionTransitionToDebug(source.lt);
                pm.Add(source.GetReachableEffect());
            }

            if (!visitedTransitions.ContainsKey(source.Name))
            {
                uncheckedReachableTransitions.Add(source.Name);
            }
        };
    }

    public void OnItemObtained(int id, string itemName, string placementName)
    {
        (RandoItem ri, RandoLocation rl) = ctx.ItemPlacements[id];
        obtainedItems.Add(id);
        AppendRandoItemToDebug(ri, rl);
        pm.Add(ri, rl);
        
    }

    public void OnPlacementPreviewed(string placementName)
    {
        previewedLocations.Add(placementName);
        uncheckedReachableLocations.Remove(placementName);
    }

    public void OnPlacementCleared(string placementName)
    {
        clearedLocations.Add(placementName);
        previewedLocations.Remove(placementName);
        uncheckedReachableLocations.Remove(placementName);
    }

    public void OnTransitionVisited(string source, string target)
    {
        visitedTransitions[source] = target;
        uncheckedReachableTransitions.Remove(source);

        LogicTransition st = lm.GetTransition(source);
        if (st.CanGet(pm))
        {
            LogicTransition tt = lm.GetTransition(target);
            if (!pm.Has(st.term))
            {
                AppendProgressionTransitionToDebug(st);
                pm.Add(st.GetReachableEffect());
            }

            AppendTransitionTargetToDebug(tt, st);
            pm.Add(tt, st);
        }
    }

    public void OnPreviewsCleared()
    {
        previewedLocations.Clear();
        Reset();
    }

    public void OnFoundTransitionsCleared()
    {
        visitedTransitions.Clear();
        uncheckedReachableTransitions.Clear();
        uncheckedReachableLocations.Clear();
        Reset();
    }

    private void AppendToDebug(string line)
    {
        ArchipelagoMapMod.Instance.LogDebug(line);
        //LogManager.Append(line + Environment.NewLine, logFileName);
    }
    
    private void AppendToFine(string line)
    {
        ArchipelagoMapMod.Instance.LogFine(line);
        //LogManager.Append(line + Environment.NewLine, logFileName);
    }

    private void AppendWaypointToDebug(LogicWaypoint w)
    {
        AppendToDebug("New reachable waypoint: " + w.Name);
    }

    private void AppendVanillaToDebug(GeneralizedPlacement v)
    {
        AppendToFine($"New reachable vanilla placement: {v.Item.Name} at {v.Location.Name}");
    }

    private void AppendRandoItemToDebug(RandoItem ri, RandoLocation rl)
    {
        try
        {
            string s = rl.Name;
        }
        catch (NullReferenceException)
        {
            
        }
        AppendToDebug($"Adding randomized item obtained from {rl.Name} to progression: {ri.Name}");
    }

    private void AppendRandoLocationToDebug(RandoLocation rl)
    {
        AppendToDebug("New reachable randomized location: " + rl.Name);
    }

    private void AppendReachableTransitionToDebug(LogicTransition lt)
    {
        AppendToDebug("New reachable randomized transition: " + lt.Name);
    }

    private void AppendProgressionTransitionToDebug(LogicTransition lt)
    {
        AppendToDebug("Adding randomized transition to progression: " + lt.Name);
    }

    private void AppendTransitionTargetToDebug(LogicTransition target, LogicTransition source)
    {
        AppendToDebug($"Adding randomized transition pair {source.Name} --> {target.Name}");
    }

    private void AppendImprovedStateToDebug(Term target, StateUnion value)
    {
        AppendToFine($"Improved {target.Name} state to {lm.StateManager.PrettyPrint(value)} via evaluation.");
    }

    private void AppendTransmittedStateToDebug(Term target, StateUnion value)
    {
        AppendToFine($"Improved {target.Name} state to {lm.StateManager.PrettyPrint(value)} via transmission.");
    }

    public bool HasVisited(string transition) => visitedTransitions.ContainsKey(transition);

    public class DelegateUpdateEntry : UpdateEntry
    {
        readonly Action<ProgressionManager> onAdd;
        readonly ILogicDef location;

        public DelegateUpdateEntry(ILogicDef location, Action<ProgressionManager> onAdd)
        {
            this.location = location;
            this.onAdd = onAdd;
        }

        public override bool CanGet(ProgressionManager pm)
        {
            return location.CanGet(pm);
        }

        public override IEnumerable<Term> GetTerms()
        {
            return location.GetTerms();
        }

        public override void OnAdd(ProgressionManager pm)
        {
            onAdd?.Invoke(pm);
        }
    }
}