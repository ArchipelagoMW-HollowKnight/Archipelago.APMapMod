﻿using ArchipelagoMapMod.Modes;
using ArchipelagoMapMod.Pathfinder;
using ArchipelagoMapMod.Settings;
using MagicUI.Elements;
using MapChanger;
using MapChanger.UI;

namespace ArchipelagoMapMod.UI;

internal class RouteText : MapUILayer
{
    internal static RouteText Instance;

    private static TextObject route;

    protected override bool Condition()
    {
        return Conditions.TransitionRandoModeEnabled()
               && (States.WorldMapOpen || States.QuickMapOpen
                                       || (!GameManager.instance.IsGamePaused() &&
                                           ArchipelagoMapMod.GS.RouteTextInGame is RouteTextInGame.NextTransitionOnly
                                               or RouteTextInGame.Show));
    }

    public override void BuildLayout()
    {
        Instance = this;

        route = UIExtensions.TextFromEdge(Root, "Unchecked", false);
    }

    public override void Update()
    {
        route.Text = GetRouteText();
    }

    private static string GetRouteText()
    {
        var text = "";

        if (RouteManager.CurrentRoute is null) return text;

        if (ArchipelagoMapMod.GS.RouteTextInGame is RouteTextInGame.NextTransitionOnly
            && !States.QuickMapOpen && !States.WorldMapOpen)
            return RouteManager.CurrentRoute.RemainingInstructions.First().ArrowedText;

        foreach (var instruction in RouteManager.CurrentRoute.RemainingInstructions)
        {
            if (text.Length > 100)
            {
                text += " -> ..." + RouteManager.CurrentRoute.RemainingInstructions.Last().ArrowedText;
                break;
            }

            text += instruction.ArrowedText;
        }

        if ((States.WorldMapOpen || States.QuickMapOpen)
            && RouteManager.CurrentRoute.GetHint() is string hint)
            text += $"\n\n{hint}";

        return text;
    }
}