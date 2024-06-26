﻿using ArchipelagoMapMod.Modes;
using ArchipelagoMapMod.Pathfinder;
using ArchipelagoMapMod.Pathfinder.Instructions;
using GlobalEnums;
using MapChanger;
using RandomizerCore.Logic;
using RCPathfinder;
using UnityEngine;

namespace ArchipelagoMapMod.Transition;

internal class TransitionTracker : HookModule
{
    private static readonly (string condition, string transition)[] waypointTransitionPairs =
    {
        ("Opened_Black_Egg_Temple", "Room_temple[door1]"),
        ("Opened_Black_Egg_Temple", "Room_Final_Boss_Atrium[left1]"),
        ("GG_Atrium", "GG_Atrium[Door_Workshop]"),
        ("GG_Workshop", "GG_Workshop[left1]")
    };

    private static readonly (string waypoint, string scene)[] waypointScenePairs =
    {
        ("Bench-Black_Egg_Temple", "Room_Final_Boss_Atrium"),
        ("Opened_Black_Egg_Temple", "Room_Final_Boss_Atrium"),
        ("Can_Stag", "Room_Town_Stag_Station"),
        ("Warp-Godhome_to_Junk_Pit", "GG_Atrium"),
        ("Warp-Junk_Pit_to_Godhome", "GG_Waterways"),
        ("GG_Workshop", "GG_Workshop"),
        ("Upper_Tram", "Room_Tram_RG"),
        ("Lower_Tram", "Room_Tram")
    };

    internal static HashSet<string> InLogicExtraTransitions { get; private set; }
    internal static HashSet<string> InLogicScenes { get; private set; } = [];
    internal static HashSet<string> VisitedAdjacentScenes { get; private set; } = [];
    internal static HashSet<string> UncheckedReachableScenes { get; private set; } = [];

    public override void OnEnterGame()
    {
        Events.OnWorldMap += Events_OnWorldMap;
        Events.OnQuickMap += Events_OnQuickMap;
    }

    public override void OnQuitToMenu()
    {
        Events.OnWorldMap -= Events_OnWorldMap;
        Events.OnQuickMap -= Events_OnQuickMap;
    }

    private void Events_OnQuickMap(GameMap arg1, MapZone arg2)
    {
        Update();
    }

    private void Events_OnWorldMap(GameMap obj)
    {
        Update();
    }

    internal static void Update()
    {
        //ArchipelagoMapMod.Instance.LogDebug("Update TransitionTracker");
        InLogicExtraTransitions = [];
        InLogicScenes = [];
        UncheckedReachableScenes = [];

        var pm = ArchipelagoMapMod.LS.TrackerData.pm;

        // Get in-logic extra transitions from waypoints
        foreach ((var waypoint, var transition) in waypointTransitionPairs)
        {
            if (pm.lm.Terms.IsTerm(waypoint) && pm.Get(waypoint) > 0)
            {
                InLogicExtraTransitions.Add(transition);
            }
        }

        // Get in-logic scenes from transitions
        foreach (var transition in TransitionData.Transitions)
        {
            if ((pm.lm.Terms.IsTerm(transition.Name) && pm.Get(transition.Name) > 0) ||
                InLogicExtraTransitions.Contains(transition.Name))
            {
                InLogicScenes.Add(transition.SceneName);
            }
        }

        // Get more in-logic scenes from waypoints
        foreach ((var waypoint, var scene) in waypointScenePairs)
        {
            if (pm.lm.Terms.IsTerm(waypoint) && pm.Get(waypoint) > 0)
            {
                InLogicScenes.Add(scene);
            }
        }

        VisitedAdjacentScenes = GetVisitedAdjacentScenes();

        // Get scenes where there are unchecked reachable transitions
        foreach (var transition in ArchipelagoMapMod.LS.TrackerData.uncheckedReachableTransitions)
        {
            if (TransitionData.GetTransitionDef(transition) is APmmTransitionDef td)
            {
                UncheckedReachableScenes.Add(td.SceneName);
            }
        }
    }

    internal static bool GetRoomActive(string scene)
    {
        if (MapChanger.Settings.CurrentMode() is TransitionNormalMode)
        {
            return Tracker.HasVisitedScene(scene) || InLogicScenes.Contains(scene);
        }

        if (MapChanger.Settings.CurrentMode() is TransitionVisitedOnlyMode)
        {
            return Tracker.HasVisitedScene(scene);
        }

        return true;
    }

    internal static Vector4 GetRoomColor(string scene)
    {
        var color = APmmColors.GetColor(APmmColorSetting.Room_Out_of_logic);

        if (InLogicScenes.Contains(scene))
        {
            color = APmmColors.GetColor(APmmColorSetting.Room_Normal);
        }

        if (VisitedAdjacentScenes.Contains(scene))
        {
            color = APmmColors.GetColor(APmmColorSetting.Room_Adjacent);
        }

        if (scene == Utils.CurrentScene())
        {
            color = APmmColors.GetColor(APmmColorSetting.Room_Current);
        }

        if (UncheckedReachableScenes.Contains(scene))
        {
            color.w = 1f;
        }

        return color;
    }

    private static HashSet<string> GetVisitedAdjacentScenes()
    {
        var scene = Utils.CurrentScene();

        if (scene is "Room_Tram")
        {
            return ["Abyss_03", "Abyss_03_b", "Abyss_03_c"];
        }

        if (scene is "Room_Tram_RG")
        {
            return ["Crossroads_46", "Crossroads_46b"];
        }

        var starts = APmmPathfinder.SD.GetPrunedStartTerms(scene);

        if (!starts.Any())
        {
            return [];
        }

        SearchParams sp = new SearchParams
        {
            StartPositions = starts,
            StartState = APmmPathfinder.SD.CurrentState,
            Destinations = new Term[0],
            MaxCost = 0.5f,
            MaxTime = float.PositiveInfinity,
            TerminationCondition = TerminationConditionType.None,
        };

        SearchState ss = new(sp);
        Algorithms.DijkstraSearch(APmmPathfinder.SD, sp, ss);

        HashSet<string> scenes = [];

        foreach (var (_, node) in ss.QueueNodes)
        {
            //ArchipelagoMapMod.Instance.LogDebug(item.Item2.PrintActions());
            if (InstructionData.GetInstructions(node).FirstOrDefault() is Instruction i)
            {
                //ArchipelagoMapMod.Instance.LogDebug(i.ArrowedText);
                if (i is TramInstruction ti)
                {
                    if (ti.Waypoint is "Lower_Tram")
                    {
                        scenes.Add("Room_Tram");
                    }

                    if (ti.Waypoint is "Upper_Tram")
                    {
                        scenes.Add("Room_Tram_RG");
                    }

                    continue;
                }

                scenes.Add(i.TargetScene);
            }
        }

        return scenes;
    }
}