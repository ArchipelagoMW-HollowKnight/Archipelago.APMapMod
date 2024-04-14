using ArchipelagoMapMod.Transition;
using RandomizerCore.Logic;
using RCPathfinder;
using System.Diagnostics;

namespace ArchipelagoMapMod.Pathfinder;

internal static class Testing
{
    private static readonly Random rng = new(0);

    internal static void LogProgressionData(APmmSearchData sd)
    {
        ArchipelagoMapMod.Instance?.LogDebug("  Logging PMs");

        foreach (var term in sd.Positions)
        {
            ArchipelagoMapMod.Instance.LogDebug($"    {term.Name}");
            ArchipelagoMapMod.Instance.LogDebug(
                $"      Reference: {ArchipelagoMapMod.LS.TrackerData.lm.Terms.IsTerm(term.Name) && ArchipelagoMapMod.LS.TrackerData.pm.Has(term)}");
            ArchipelagoMapMod.Instance.LogDebug($"      Local: {sd.LocalPM.Has(term)}");
        }
    }

    internal static void DebugActions(APmmSearchData sd)
    {
        sd.UpdateProgression();

        foreach (var kvp in sd.ActionLookup)
        {
            ArchipelagoMapMod.Instance?.LogDebug($"  Testing actions from {kvp.Key.Name}");

            foreach (var action in kvp.Value)
            {
                sd.LocalPM.SetState(kvp.Key, sd.CurrentState);

                ArchipelagoMapMod.Instance?.LogDebug(
                    $"    {action.DebugString}, {action.Cost}: {action.TryDo(sd.LocalPM, kvp.Key, sd.CurrentState, out var _)}");

                sd.LocalPM.SetState(kvp.Key, null);
            }
        }
    }

    internal static void SingleStartDestinationTest(APmmSearchData sd)
    {
        var inLogicTransitions = sd.Positions.Where(p => TransitionData.GetTransitionDef(p.Name) is not null
                                                         && (ArchipelagoMapMod.LS.TrackerData.pm.lm.GetTerm(p.Name) is null ||
                                                             ArchipelagoMapMod.LS.TrackerData.pm.Has(p))).ToArray();

        sd.UpdateProgression();

        SearchParams sp = new SearchParams
        {
            StartPositions = null,
            StartState = APmmPathfinder.SD.CurrentState,
            Destinations = null,
            MaxCost = 1000f,
            MaxTime = float.PositiveInfinity,
            TerminationCondition = TerminationConditionType.Any,
        };

        ArchipelagoMapMod.Instance?.LogDebug("Starting SingleStartDestinationTest:");

        var globalSW = Stopwatch.StartNew();
        var sw = Stopwatch.StartNew();

        var testCount = 100;
        var successes = 0;

        for (var i = 0; i < testCount; i++)
        {
            var start = GetRandomTerm(inLogicTransitions);
            var destination = GetRandomTerm(inLogicTransitions);

            sp.StartPositions = new StartPosition[] {new(start.Name, start, 0f)};
            sp.Destinations = new[] {destination};

            SearchState ss = new(sp);

            ArchipelagoMapMod.Instance?.LogDebug($"Trying {start} -> ? -> {destination}");

            sw.Restart();

            if (DoTest(sd, sp, ss))
            {
                foreach (var instruction in InstructionData.GetInstructions(ss.NewResultNodes[0]))
                {
                    ArchipelagoMapMod.Instance?.LogDebug($"    {instruction.ArrowedText}");
                }

                successes++;
            }
            else
            {
                ArchipelagoMapMod.Instance?.LogDebug($"{start} to {destination} failed");
            }

            sw.Stop();

            var localElapsedMS = sw.ElapsedTicks * 1000f / Stopwatch.Frequency;
            ArchipelagoMapMod.Instance?.LogDebug(
                $"Explored {ss.NodesPopped} nodes in {localElapsedMS} ms. Average nodes/ms: {ss.NodesPopped / localElapsedMS}");
        }

        globalSW.Stop();

        var globalElapsedMS = globalSW.ElapsedTicks * 1000f / Stopwatch.Frequency;

        ArchipelagoMapMod.Instance?.LogDebug($"Total computation time: {globalElapsedMS} ms");
        ArchipelagoMapMod.Instance?.LogDebug($"Total successes: {successes}/{testCount}");
        ArchipelagoMapMod.Instance?.LogDebug($"Average serarch time: {globalElapsedMS / testCount} ms");
    }

    internal static void SceneToSceneTest(APmmSearchData sd)
    {
        TransitionTracker.Update();

        sd.UpdateProgression();

        SearchParams sp = new SearchParams
        {
            StartPositions = null,
            StartState = APmmPathfinder.SD.CurrentState,
            Destinations = null,
            MaxCost = 100f,
            MaxTime = float.PositiveInfinity,
            TerminationCondition = TerminationConditionType.Any,
        };

        ArchipelagoMapMod.Instance?.LogDebug("Starting SceneToSceneTest:");

        var globalSW = Stopwatch.StartNew();
        var sw = Stopwatch.StartNew();

        var testCount = 100;

        for (var i = 0; i < testCount; i++)
        {
            HashSet<Route> routes = [];

            var startScene = TransitionTracker.InLogicScenes.ElementAt(rng.Next(TransitionTracker.InLogicScenes.Count));
            var destScene = TransitionTracker.InLogicScenes.ElementAt(rng.Next(TransitionTracker.InLogicScenes.Count));

            sp.StartPositions = GetPrunedTransitions(sd, startScene).Select(t => new StartPosition(t.Name, t, 0f))
                .ToArray();
            sp.Destinations = GetPrunedTransitions(sd, destScene).ToArray();

            if (!sp.StartPositions.Any() || !sp.Destinations.Any())
            {
                ArchipelagoMapMod.Instance?.LogDebug($"Invalid scenes {startScene} or {destScene}. Skipping");
                continue;
            }

            //List<(string, Term)> benchStartWarps = BenchwarpInterop.BenchNames.Values
            //    .Where(sd.PositionLookup.ContainsKey)
            //    .Select(b => ("benchStart", sd.PositionLookup[b])).ToList();

            //if (sd.StartTerm is not null)
            //{
            //    benchStartWarps.Add(("benchStart", sd.StartTerm));
            //}

            //sp.StartPositions = sp.StartPositions.Concat(benchStartWarps).ToArray();

            SearchState ss = new(sp);

            ArchipelagoMapMod.Instance?.LogDebug($"Trying {startScene} -> ? -> {destScene}");

            sw.Restart();

            while (DoTest(sd, sp, ss))
            {
                Route route = new(ss.NewResultNodes[0]);
                if (routes.Contains(route))
                {
                    ArchipelagoMapMod.Instance?.LogDebug("    Duplicate route detected");
                }
                else
                {
                    routes.Add(route);
                    foreach (var instruction in route.RemainingInstructions)
                    {
                        ArchipelagoMapMod.Instance?.LogDebug($"    {instruction.ArrowedText}");
                    }
                }
            }

            sw.Stop();

            var localElapsedMS = sw.ElapsedTicks * 1000f / Stopwatch.Frequency;
            ArchipelagoMapMod.Instance?.LogDebug(
                $"Explored {ss.NodesPopped} nodes in {localElapsedMS} ms. Average nodes/ms: {ss.NodesPopped / localElapsedMS}");
        }

        globalSW.Stop();

        var globalElapsedMS = globalSW.ElapsedTicks * 1000f / Stopwatch.Frequency;

        ArchipelagoMapMod.Instance?.LogDebug($"Total computation time: {globalElapsedMS} ms");
        ArchipelagoMapMod.Instance?.LogDebug($"Average serarch time: {globalElapsedMS / testCount} ms");
    }

    /// <summary>
    ///     Removes transitions that are immediately accesible from another transition in the same scene.
    /// </summary>
    private static Term[] GetPrunedTransitions(APmmSearchData sd, string scene)
    {
        if (!sd.TransitionTermsByScene.TryGetValue(scene, out var transitions))
        {
            return new Term[] { };
        }

        SearchParams sp = new SearchParams
        {
            StartPositions = transitions.Select(t => new StartPosition(t.Name, t, 0f)).ToArray(),
            StartState = sd.CurrentState,
            Destinations = transitions.ToArray(),
            MaxCost = 1f,
            MaxTime = float.PositiveInfinity,
            TerminationCondition = TerminationConditionType.Any
        };

        SearchState ss = new(sp);

        Algorithms.DijkstraSearch(sd, sp, ss);

        List<Node> nodes =
            new(ss.ResultNodes.Where(n => n.Depth > 0 && n.StartPosition.Term != n.Actions.Last().Destination));

        HashSet<Term> prunedTransitions = new(transitions);

        foreach (var transition in transitions)
        {
            if (!prunedTransitions.Contains(transition))
            {
                continue;
            }

            foreach (var transition2 in new List<Term>(prunedTransitions))
            {
                if (nodes.Any(n => n.StartPosition.Term == transition && n.Actions.Last().Destination == transition2))
                {
                    prunedTransitions.Remove(transition2);
                }
            }
        }

        return prunedTransitions.ToArray();
    }

    private static bool DoTest(SearchData sd, SearchParams sp, SearchState search)
    {
        if (Algorithms.DijkstraSearch(sd, sp, search))
        {
            ArchipelagoMapMod.Instance?.LogDebug("  Success!");
            return true;
        }

        if (search.QueueNodes.Count > 0)
        {
            ArchipelagoMapMod.Instance?.LogDebug("  Search terminated after reaching max cost.");
        }
        else
        {
            ArchipelagoMapMod.Instance?.LogDebug("  Search exhausted with no route found.");
        }

        return false;
    }

    private static Term GetRandomTerm(Term[] terms)
    {
        return terms[rng.Next(terms.Length)];
    }
}