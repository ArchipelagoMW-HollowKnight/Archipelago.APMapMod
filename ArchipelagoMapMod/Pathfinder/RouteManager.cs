using ArchipelagoMapMod.Pathfinder.Instructions;
using ArchipelagoMapMod.Settings;
using ArchipelagoMapMod.Transition;
using ArchipelagoMapMod.UI;
using MapChanger;
using RCPathfinder;
using Events = ItemChanger.Events;

namespace ArchipelagoMapMod.Pathfinder;

internal class RouteManager : HookModule
{
    private static SearchParams _sp;
    private static SearchState _ss;
    private static HashSet<Route> _routes;

    internal static Route CurrentRoute { get; private set; }
    internal static string StartScene { get; private set; }
    internal static string FinalScene { get; private set; }
    internal static bool Reevaluated { get; private set; }

    public override void OnEnterGame()
    {
        Events.OnBeginSceneTransition += CheckRoute;
        MapChanger.Settings.OnSettingChanged += ResetRoute;
    }

    public override void OnQuitToMenu()
    {
        ResetRoute();

        Events.OnBeginSceneTransition -= CheckRoute;
        MapChanger.Settings.OnSettingChanged -= ResetRoute;
    }

    internal static bool TryGetNextRouteTo(string scene)
    {
        APmmPathfinder.SD.UpdateProgression();

        // Reset
        if (!CanCycleRoute(scene))
        {
            StartScene = Utils.CurrentScene();
            FinalScene = scene;

            _sp = new SearchParams(
                APmmPathfinder.SD.GetPrunedStartTerms(StartScene),
                APmmPathfinder.SD.CurrentState,
                APmmPathfinder.SD.GetTransitionTerms(FinalScene),
                1000f,
                TerminationConditionType.Any
            );

            if (Interop.HasBenchwarp() && ArchipelagoMapMod.GS.PathfinderBenchwarp)
                _sp.StartPositions = _sp.StartPositions.Concat(GetBenchStartWarps()).ToArray();

            if (TryGetDreamGateStart(out var dreamGateStart))
                _sp.StartPositions = _sp.StartPositions.Append(dreamGateStart).ToArray();

            if (!_sp.StartPositions.Any() || !_sp.Destinations.Any())
            {
                ResetRoute();
                return false;
            }

            _ss = new SearchState(_sp);
            _routes = new HashSet<Route>();
        }

        Reevaluated = false;

        while (Algorithms.DijkstraSearch(APmmPathfinder.SD, _sp, _ss))
        {
            Route route = new(_ss.NewResultNodes[0]);

            //ArchipelagoMapMod.Instance.LogDebug($"Found a route from {route.Node.StartPosition} to {route.Destination}:");
            //ArchipelagoMapMod.Instance.LogDebug(route.Node.PrintActions());

            if (!route.RemainingInstructions.Any()
                || route.RemainingInstructions.First().Text == route.RemainingInstructions.Last().TargetTransition
                || _routes.Contains(route)) continue;

            _routes.Add(route);
            CurrentRoute = route;
            RouteCompass.Update();
            UpdateRouteUI();
            return true;
        }

        // Search exhausted, clear search state and reset
        ResetRoute();
        return false;
    }

    internal static bool TryReevaluateRoute(ItemChanger.Transition transition)
    {
        APmmPathfinder.SD.UpdateProgression();

        StartScene = transition.SceneName;
        var destination = CurrentRoute.Destination;

        _sp = new SearchParams(
            null,
            APmmPathfinder.SD.CurrentState,
            new[] {destination},
            1000f,
            TerminationConditionType.Any
        );

        if (TransitionData.GetTransitionDef(transition.ToString()) is APmmTransitionDef td
            && APmmPathfinder.SD.PositionLookup.TryGetValue(td.Name, out var start))
            _sp.StartPositions = new StartPosition[] {new(start.Name, start, 0f)};
        else
            _sp.StartPositions = APmmPathfinder.SD.GetPrunedStartTerms(StartScene);

        if (Interop.HasBenchwarp() && ArchipelagoMapMod.GS.PathfinderBenchwarp)
            _sp.StartPositions = _sp.StartPositions.Concat(GetBenchStartWarps(true)).ToArray();

        if (TryGetDreamGateStart(out var dreamGateStart, transition))
            _sp.StartPositions = _sp.StartPositions.Append(dreamGateStart).ToArray();

        if (!_sp.StartPositions.Any() || !_sp.Destinations.Any())
        {
            ResetRoute();
            return false;
        }

        _ss = new SearchState(_sp);

        if (Algorithms.DijkstraSearch(APmmPathfinder.SD, _sp, _ss))
        {
            Route route = new(_ss.NewResultNodes[0]);

            //foreach (AbstractAction action in route.Node.Actions)
            //{
            //    ArchipelagoMapMod.Instance.LogDebug($"{action.DebugName}");
            //}

            if (!route.RemainingInstructions.Any())
            {
                ResetRoute();
                return false;
            }

            CurrentRoute = route;
            Reevaluated = true;
            return true;
        }

        ResetRoute();
        return false;
    }

    internal static void CheckRoute(ItemChanger.Transition lastTransition)
    {
        //ArchipelagoMapMod.Instance.LogDebug($"Last transition: {lastTransition}");

        if (CurrentRoute is null) return;

        var instruction = CurrentRoute.RemainingInstructions.First();

        if (instruction.IsInProgress(lastTransition)) return;

        if (instruction.IsFinished(lastTransition))
        {
            CurrentRoute.RemainingInstructions.RemoveAt(0);
            if (!CurrentRoute.RemainingInstructions.Any())
            {
                ResetRoute();
                return;
            }

            UpdateRouteUI();
            return;
        }

        // The transition doesn't match the route
        switch (ArchipelagoMapMod.GS.WhenOffRoute)
        {
            case OffRouteBehaviour.Cancel:
                ResetRoute();
                break;
            case OffRouteBehaviour.Reevaluate:
                TryReevaluateRoute(lastTransition);
                UpdateRouteUI();
                break;
        }
    }

    internal static void ResetRoute()
    {
        CurrentRoute = null;
        StartScene = null;
        FinalScene = null;
        Reevaluated = false;
        _sp = null;
        _routes = null;

        RouteCompass.Update();
        UpdateRouteUI();
    }

    private static void UpdateRouteUI()
    {
        RouteText.Instance.Update();
        RouteSummaryText.Instance.Update();
        SelectionPanels.UpdateRoomPanel();
    }

    internal static bool TryGetBenchwarpKey(out APmmBenchKey key)
    {
        if (CurrentRoute is not null &&
            CurrentRoute.RemainingInstructions.First().IsOrIsSubclassInstanceOf<BenchwarpInstruction>())
        {
            key = ((BenchwarpInstruction) CurrentRoute.RemainingInstructions.First()).BenchKey;
            return true;
        }

        key = default;
        return false;
    }

    internal static bool CanCycleRoute(string scene)
    {
        return !Reevaluated
               && Utils.CurrentScene() == StartScene
               && scene == FinalScene
               && CurrentRoute is not null
               && CurrentRoute.RemainingInstructions.Count() == CurrentRoute.TotalInstructionCount;
    }

    /// <summary>
    ///     May exclude a bench/start warp based on the transition and last respawn marker.
    /// </summary>
    private static List<StartPosition> GetBenchStartWarps(bool removeLastWarp = false)
    {
        APmmBenchKey key = new(Utils.CurrentScene(), PlayerData.instance.GetString("respawnMarkerName"));
        BenchwarpInterop.BenchNames.TryGetValue(key, out var lastWarp);

        var benchStarts = BenchwarpInterop.GetVisitedBenchNames()
            .Where(APmmPathfinder.SD.PositionLookup.ContainsKey)
            .Select(b => new StartPosition("benchStart", APmmPathfinder.SD.PositionLookup[b], 1f)).ToList();

        if (removeLastWarp) benchStarts.RemoveAll(b => b.Term.Name == lastWarp);

        if (APmmPathfinder.SD.StartTerm is not null &&
            (!removeLastWarp || lastWarp != BenchwarpInterop.BENCH_WARP_START))
            benchStarts.Add(new StartPosition("benchStart", APmmPathfinder.SD.StartTerm, 1f));

        return benchStarts;
    }

    private static bool TryGetDreamGateStart(out StartPosition dreamGateStart,
        ItemChanger.Transition transition = default)
    {
        if (DreamgateTracker.DreamgateTiedTransition is null
            || (transition != default && transition.GateName is "dreamGate")
            || !APmmPathfinder.SD.PositionLookup.TryGetValue(DreamgateTracker.DreamgateTiedTransition, out var term))
        {
            dreamGateStart = default;
            return false;
        }

        dreamGateStart = new StartPosition("dreamGate", term, 1f);
        return true;
    }
}