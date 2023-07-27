using ArchipelagoMapMod.Modes;
using UnityEngine;

namespace ArchipelagoMapMod.UI;

internal class CompassText : ControlPanelText
{
    private protected override string Name => "Compass";

    private protected override bool ActiveCondition()
    {
        return ArchipelagoMapMod.GS.ControlPanelOn && Conditions.TransitionRandoModeEnabled();
    }

    private protected override Vector4 GetColor()
    {
        return ArchipelagoMapMod.GS.ShowRouteCompass
            ? APmmColors.GetColor(APmmColorSetting.UI_On)
            : APmmColors.GetColor(APmmColorSetting.UI_Neutral);
    }

    private protected override string GetText()
    {
        var text = "Show route compass (Ctrl-C): ";
        return text + (ArchipelagoMapMod.GS.ShowRouteCompass ? "On" : "Off");
    }
}