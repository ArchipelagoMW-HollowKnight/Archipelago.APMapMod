﻿using ArchipelagoMapMod.Modes;
using ArchipelagoMapMod.Settings;
using UnityEngine;
using L = RandomizerMod.Localization;

namespace ArchipelagoMapMod.UI;

internal class ShowRouteInGameText : ControlPanelText
{
    private protected override string Name => "Show Route In Game";

    private protected override bool ActiveCondition()
    {
        return ArchipelagoMapMod.GS.ControlPanelOn && Conditions.TransitionRandoModeEnabled();
    }

    private protected override Vector4 GetColor()
    {
        return ArchipelagoMapMod.GS.RouteTextInGame switch
        {
            RouteTextInGame.Hide => apmmColors.GetColor(apmmColorSetting.UI_Neutral),
            RouteTextInGame.Show => apmmColors.GetColor(apmmColorSetting.UI_On),
            RouteTextInGame.NextTransitionOnly => apmmColors.GetColor(apmmColorSetting.UI_On),
            _ => apmmColors.GetColor(apmmColorSetting.UI_Neutral)
        };
    }

    private protected override string GetText()
    {
        var text = $"{L.Localize("Show route in-game")} (Ctrl-G): ";

        return ArchipelagoMapMod.GS.RouteTextInGame switch
        {
            RouteTextInGame.Hide => text + L.Localize("Off"),
            RouteTextInGame.Show => text + L.Localize("Full"),
            RouteTextInGame.NextTransitionOnly => text + L.Localize("Next transition only"),
            _ => text
        };
    }
}