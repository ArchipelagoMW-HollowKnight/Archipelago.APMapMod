using System.Collections.ObjectModel;
using MapChanger;
using L = RandomizerMod.Localization;
using RD = RandomizerMod.RandomizerData.Data;
using RM = RandomizerMod.RandomizerMod;

namespace ArchipelagoMapMod.Transition;

internal class TransitionData : HookModule
{
    private static Dictionary<string, apmmTransitionDef> _vanillaTransitions;
    private static Dictionary<string, apmmTransitionDef> _randomizedTransitions;
    private static Dictionary<string, string> _placements;

    internal static (string location, string item)[] ExtraVanillaTransitions { get; } =
    {
        ("Room_temple[door1]", "Room_Final_Boss_Atrium[left1]"),
        ("Room_Final_Boss_Atrium[left1]", "Room_temple[door1]"),
        ("GG_Atrium[Door_Workshop]", "GG_Workshop[left1]"),
        ("GG_Workshop[left1]", "GG_Atrium[Door_Workshop]")
    };

    internal static ReadOnlyCollection<apmmTransitionDef> Transitions { get; private set; }

    public static ReadOnlyDictionary<string, string> Placements { get; private set; }

    public override void OnEnterGame()
    {
        _vanillaTransitions = new Dictionary<string, apmmTransitionDef>();
        _randomizedTransitions = new Dictionary<string, apmmTransitionDef>();
        _placements = new Dictionary<string, string>();

        // Add transition placements
        foreach ((var location, var item) in RM.RS.Context.Vanilla.Select(p => (p.Location.Name, p.Item.Name))
                     .Concat(ExtraVanillaTransitions))
        {
            if (RD.IsTransition(location) && RD.IsTransition(item))
            {
                _vanillaTransitions[location] = new apmmTransitionDef(RD.GetTransitionDef(location));
                _vanillaTransitions[item] = new apmmTransitionDef(RD.GetTransitionDef(item));
                _placements[location] = item;
                continue;
            }

            // Connection-provided vanilla transitions, including extra ones
            if (apmmTransitionDef.TryMake(location, out var locationTD)
                && apmmTransitionDef.TryMake(item, out var itemTD)
                && locationTD is not null && itemTD is not null)
            {
                _vanillaTransitions[location] = locationTD;
                _vanillaTransitions[item] = itemTD;
                _placements[location] = item;
            }
        }

        if (RM.RS.Context.transitionPlacements is not null)
            foreach ((var source, var target) in RM.RS.Context.transitionPlacements.Select(p =>
                         (p.Source.TransitionDef, p.Target.TransitionDef)))
            {
                _randomizedTransitions[source.Name] = new apmmTransitionDef(source);
                _randomizedTransitions[target.Name] = new apmmTransitionDef(target);
                _placements[source.Name] = target.Name;
            }

        Placements = new ReadOnlyDictionary<string, string>(_placements);

        Transitions =
            new ReadOnlyCollection<apmmTransitionDef>(_vanillaTransitions.Values.Concat(_randomizedTransitions.Values)
                .ToArray());
    }

    public override void OnQuitToMenu()
    {
        _vanillaTransitions = null;
        _randomizedTransitions = null;
        _placements = null;
    }

    public static bool IsTransitionRando()
    {
        return _randomizedTransitions.Any();
    }

    public static bool IsVanillaTransition(string transition)
    {
        return _vanillaTransitions.ContainsKey(transition);
    }

    public static bool IsRandomizedTransition(string transition)
    {
        return _randomizedTransitions.ContainsKey(transition);
    }

    internal static bool IsVanillaOrCheckedTransition(string transition)
    {
        return RM.RS.TrackerData.HasVisited(transition)
               || (_vanillaTransitions.ContainsKey(transition)
                   && (RM.RS.TrackerData.lm.GetTerm(transition) is null || RM.RS.TrackerData.pm.Get(transition) > 0));
    }

    public static bool TryGetScene(string str, out string scene)
    {
        if (GetTransitionDef(str) is apmmTransitionDef td)
        {
            scene = td.SceneName;
            return true;
        }

        scene = default;
        return false;
    }

    public static apmmTransitionDef GetTransitionDef(string str)
    {
        if (_vanillaTransitions.TryGetValue(str, out var def)) return def;
        if (_randomizedTransitions.TryGetValue(str, out def)) return def;

        return null;
    }

    internal static string GetUncheckedVisited(string scene)
    {
        var text = "";

        var uncheckedTransitions = RM.RS.TrackerData.uncheckedReachableTransitions
            .Where(t => TryGetScene(t, out var s) && s == scene);

        if (uncheckedTransitions.Any())
        {
            text += $"{L.Localize("Unchecked")}:";

            foreach (var transition in uncheckedTransitions)
            {
                if (GetTransitionDef(transition) is not apmmTransitionDef td) continue;

                text += "\n";

                if (!RM.RS.TrackerDataWithoutSequenceBreaks.uncheckedReachableTransitions.Contains(transition))
                    text += "*";

                text += td.DoorName;
            }
        }

        var visitedTransitions = RM.RS.TrackerData.visitedTransitions
            .Where(t => TryGetScene(t.Key, out var s) && s == scene)
            .ToDictionary(t => GetTransitionDef(t.Key), t => GetTransitionDef(t.Value));

        text += BuildTransitionStringList(visitedTransitions, "Visited", false, text != "");

        var visitedTransitionsTo = RM.RS.TrackerData.visitedTransitions
            .Where(t => TryGetScene(t.Value, out var s) && s == scene)
            .ToDictionary(t => GetTransitionDef(t.Key), t => GetTransitionDef(t.Value));

        // Display only one-way transitions in coupled rando
        if (RM.RS.GenerationSettings.TransitionSettings.Coupled)
            visitedTransitionsTo = visitedTransitionsTo.Where(t => !visitedTransitions.ContainsKey(t.Value))
                .ToDictionary(t => t.Key, t => t.Value);

        text += BuildTransitionStringList(visitedTransitionsTo, "Visited to", true, text != "");

        var vanillaTransitions = RM.RS.Context.Vanilla
            .Where(t => RD.IsTransition(t.Location.Name)
                        && TryGetScene(t.Location.Name, out var s) && s == scene)
            .ToDictionary(t => GetTransitionDef(t.Location.Name), t => GetTransitionDef(t.Item.Name));


        text += BuildTransitionStringList(vanillaTransitions, "Vanilla", false, text != "");

        var vanillaTransitionsTo = RM.RS.Context.Vanilla
            .Where(t => RD.IsTransition(t.Location.Name)
                        && TryGetScene(t.Item.Name, out var s) && s == scene
                        && !vanillaTransitions.Keys.Any(td => td.Name == t.Item.Name))
            .ToDictionary(t => GetTransitionDef(t.Location.Name), t => GetTransitionDef(t.Item.Name));

        text += BuildTransitionStringList(vanillaTransitionsTo, "Vanilla to", true, text != "");

        return text;
    }

    private static string BuildTransitionStringList(Dictionary<apmmTransitionDef, apmmTransitionDef> transitions,
        string subtitle, bool to, bool addNewLines)
    {
        var text = "";

        if (!transitions.Any()) return text;

        if (addNewLines) text += "\n\n";

        text += $"{L.Localize(subtitle)}:";

        foreach (var kvp in transitions)
        {
            text += "\n";

            if (RM.RS.TrackerDataWithoutSequenceBreaks.outOfLogicVisitedTransitions.Contains(kvp.Key.Name)) text += "*";

            if (to)
                text += $"{kvp.Key.Name} -> {kvp.Value.DoorName}";
            else
                text += $"{kvp.Key.DoorName} -> {kvp.Value.Name}";
        }

        return text;
    }
}