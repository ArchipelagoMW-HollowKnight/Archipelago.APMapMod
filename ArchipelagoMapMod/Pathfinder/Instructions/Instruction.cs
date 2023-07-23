﻿using ItemChanger.Extensions;
using MapChanger;
using UnityEngine;

namespace ArchipelagoMapMod.Pathfinder.Instructions;

internal class Instruction
{
    private static Dictionary<string, Dictionary<string, string>> compassObjOverrides;

    private static readonly string transitionGatePrefix = "_Transition Gates/";

    internal Instruction(string text, string targetTransition)
    {
        Text = text;

        TargetTransition = targetTransition;

        if (compassObjOverrides.TryGetValue(text, out var objs)) CompassObjects = objs;
    }

    internal string TargetScene => TargetTransition.Split('[')[0];

    /// <summary>
    ///     The transition that indicates the instruction has been performed.
    /// </summary>
    internal string TargetTransition { get; }

    internal string ArrowedText => $" -> {Text.ToCleanName()}";

    internal string Text { get; }

    /// <summary>
    ///     The path to objects per scene that the compass should point to.
    /// </summary>
    internal Dictionary<string, string> CompassObjects { get; private protected init; }

    internal static void LoadCompssObjOverrides()
    {
        compassObjOverrides = JsonUtil.DeserializeFromAssembly<Dictionary<string, Dictionary<string, string>>>(
            ArchipelagoMapMod.Assembly, "ArchipelagoMapMod.Resources.Pathfinder.Compass.compassObjOverrides.json");
    }

    internal virtual bool IsInProgress(ItemChanger.Transition lastTransition)
    {
        return false;
    }

    internal virtual bool IsFinished(ItemChanger.Transition lastTransition)
    {
        return TargetTransition == lastTransition.ToString();
    }

    internal bool TryGetCompassGO(out GameObject go)
    {
        if (CompassObjects is not null && CompassObjects.TryGetValue(Utils.CurrentScene(), out var objPath))
        {
            if (UnityExtensions.FindGameObject(UnityEngine.SceneManagement.SceneManager.GetActiveScene(), objPath) is
                GameObject compassGO)
            {
                go = compassGO;
                return true;
            }

            if (UnityExtensions.FindGameObject(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),
                    $"{transitionGatePrefix}{objPath}") is GameObject compassGO2)
            {
                go = compassGO2;
                return true;
            }

            ArchipelagoMapMod.Instance.LogWarn($"Couldn't find object called {objPath} in {Utils.CurrentScene()}");
        }

        go = null;
        return false;
    }
}