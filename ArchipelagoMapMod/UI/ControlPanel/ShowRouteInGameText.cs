using ArchipelagoMapMod.Modes;
using ArchipelagoMapMod.Settings;
using UnityEngine;

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
            RouteTextInGame.Hide => APmmColors.GetColor(APmmColorSetting.UI_Neutral),
            RouteTextInGame.Show => APmmColors.GetColor(APmmColorSetting.UI_On),
            RouteTextInGame.NextTransitionOnly => APmmColors.GetColor(APmmColorSetting.UI_On),
            _ => APmmColors.GetColor(APmmColorSetting.UI_Neutral)
        };
    }

    private protected override string GetText()
    {
        var text = "Show route in-game (Ctrl-G): ";

        return ArchipelagoMapMod.GS.RouteTextInGame switch
        {
            RouteTextInGame.Hide => text + "Off",
            RouteTextInGame.Show => text + "Full",
            RouteTextInGame.NextTransitionOnly => text + "Next transition only",
            _ => text
        };
    }
}