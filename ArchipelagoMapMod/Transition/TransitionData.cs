using ArchipelagoMapMod.RC;
using MapChanger;
using System.Collections.ObjectModel;
using RD = ArchipelagoMapMod.RandomizerData.Data;
namespace ArchipelagoMapMod.Transition;

internal class TransitionData : HookModule
{
    private static Dictionary<string, APmmTransitionDef> _vanillaTransitions;
    private static Dictionary<string, APmmTransitionDef> _randomizedTransitions;
    private static Dictionary<string, string> _placements;

    internal static (string location, string item)[] ExtraVanillaTransitions { get; } =
    {
        ("Room_temple[door1]", "Room_Final_Boss_Atrium[left1]"),
        ("Room_Final_Boss_Atrium[left1]", "Room_temple[door1]"),
        ("GG_Atrium[Door_Workshop]", "GG_Workshop[left1]"),
        ("GG_Workshop[left1]", "GG_Atrium[Door_Workshop]")
    };

    internal static ReadOnlyCollection<APmmTransitionDef> Transitions { get; private set; }

    public static ReadOnlyDictionary<string, string> Placements { get; private set; }

    public override void OnEnterGame()
    {
        _vanillaTransitions = [];
        _randomizedTransitions = [];
        _placements = [];

        // Add transition placements
        foreach ((var location, var item) in APLogicSetup.Context.Vanilla.Select(p => (p.Location.Name, p.Item.Name))
                     .Concat(ExtraVanillaTransitions))
        {
            if (RD.IsTransition(location) && RD.IsTransition(item))
            {
                _vanillaTransitions[location] = new APmmTransitionDef(RD.GetTransitionDef(location));
                _vanillaTransitions[item] = new APmmTransitionDef(RD.GetTransitionDef(item));
                _placements[location] = item;
                continue;
            }

            // Connection-provided vanilla transitions, including extra ones
            if (APmmTransitionDef.TryMake(location, out var locationTD)
                && APmmTransitionDef.TryMake(item, out var itemTD)
                && locationTD is not null && itemTD is not null)
            {
                _vanillaTransitions[location] = locationTD;
                _vanillaTransitions[item] = itemTD;
                _placements[location] = item;
            }
        }

        if (APLogicSetup.Context.TransitionPlacements is not null)
        {
            foreach ((var source, var target) in APLogicSetup.Context.TransitionPlacements.Select(p =>
                         (p.Source.TransitionDef, p.Target.TransitionDef)))
            {
                _randomizedTransitions[source.Name] = new APmmTransitionDef(source);
                _randomizedTransitions[target.Name] = new APmmTransitionDef(target);
                _placements[source.Name] = target.Name;
            }
        }

        Placements = new ReadOnlyDictionary<string, string>(_placements);

        Transitions =
            new ReadOnlyCollection<APmmTransitionDef>(_vanillaTransitions.Values.Concat(_randomizedTransitions.Values)
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
        return ArchipelagoMapMod.LS.TrackerData.HasVisited(transition)
               || (_vanillaTransitions.ContainsKey(transition)
                   && (ArchipelagoMapMod.LS.TrackerData.lm.GetTerm(transition) is null || ArchipelagoMapMod.LS.TrackerData.pm.Get(transition) > 0));
    }

    public static bool TryGetScene(string str, out string scene)
    {
        if (GetTransitionDef(str) is APmmTransitionDef td)
        {
            scene = td.SceneName;
            return true;
        }

        scene = default;
        return false;
    }

    public static APmmTransitionDef GetTransitionDef(string str)
    {
        if (_vanillaTransitions.TryGetValue(str, out var def))
        {
            return def;
        }

        if (_randomizedTransitions.TryGetValue(str, out def))
        {
            return def;
        }

        return null;
    }

    internal static string GetUncheckedVisited(string scene)
    {
        var text = "";

        var uncheckedTransitions = ArchipelagoMapMod.LS.TrackerData.uncheckedReachableTransitions
            .Where(t => TryGetScene(t, out var s) && s == scene);

        if (uncheckedTransitions.Any())
        {
            text += "Unchecked:";

            foreach (var transition in uncheckedTransitions)
            {
                if (GetTransitionDef(transition) is not APmmTransitionDef td)
                {
                    continue;
                }

                text += "\n";

                text += td.DoorName;
            }
        }

        var visitedTransitions = ArchipelagoMapMod.LS.TrackerData.visitedTransitions
            .Where(t => TryGetScene(t.Key, out var s) && s == scene)
            .ToDictionary(t => GetTransitionDef(t.Key), t => GetTransitionDef(t.Value));

        text += BuildTransitionStringList(visitedTransitions, "Visited", false, text != "");

        var visitedTransitionsTo = ArchipelagoMapMod.LS.TrackerData.visitedTransitions
            .Where(t => TryGetScene(t.Value, out var s) && s == scene)
            .ToDictionary(t => GetTransitionDef(t.Key), t => GetTransitionDef(t.Value));
        
        // Display only one-way transitions in coupled rando
        if (APLogicSetup.Context.GenerationSettings.TransitionSettings.Coupled)
        {
            visitedTransitionsTo = visitedTransitionsTo.Where(t => !visitedTransitions.ContainsKey(t.Value))
                .ToDictionary(t => t.Key, t => t.Value);
        }

        text += BuildTransitionStringList(visitedTransitionsTo, "Visited to", true, text != "");

        var vanillaTransitions = APLogicSetup.Context.Vanilla
            .Where(t => RD.IsTransition(t.Location.Name)
                        && TryGetScene(t.Location.Name, out var s) && s == scene)
            .ToDictionary(t => GetTransitionDef(t.Location.Name), t => GetTransitionDef(t.Item.Name));


        text += BuildTransitionStringList(vanillaTransitions, "Vanilla", false, text != "");

        var vanillaTransitionsTo = APLogicSetup.Context.Vanilla
            .Where(t => RD.IsTransition(t.Location.Name)
                        && TryGetScene(t.Item.Name, out var s) && s == scene
                        && !vanillaTransitions.Keys.Any(td => td.Name == t.Item.Name))
            .ToDictionary(t => GetTransitionDef(t.Location.Name), t => GetTransitionDef(t.Item.Name));

        text += BuildTransitionStringList(vanillaTransitionsTo, "Vanilla to", true, text != "");

        return text;
    }

    private static string BuildTransitionStringList(Dictionary<APmmTransitionDef, APmmTransitionDef> transitions,
        string subtitle, bool to, bool addNewLines)
    {
        var text = "";

        if (!transitions.Any())
        {
            return text;
        }

        if (addNewLines)
        {
            text += "\n\n";
        }

        text += $"{subtitle}:";

        foreach (var kvp in transitions)
        {
            text += "\n";

            if (to)
            {
                text += $"{kvp.Key.Name} -> {kvp.Value.DoorName}";
            }
            else
            {
                text += $"{kvp.Key.DoorName} -> {kvp.Value.Name}";
            }
        }

        return text;
    }
}