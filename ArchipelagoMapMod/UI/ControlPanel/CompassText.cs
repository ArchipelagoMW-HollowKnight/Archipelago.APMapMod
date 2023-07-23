using ArchipelagoMapMod.Modes;
using UnityEngine;
using L = RandomizerMod.Localization;

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
            ? apmmColors.GetColor(apmmColorSetting.UI_On)
            : apmmColors.GetColor(apmmColorSetting.UI_Neutral);
    }

    private protected override string GetText()
    {
        var text = $"{L.Localize("Show route compass")} (Ctrl-C): ";
        return text + (ArchipelagoMapMod.GS.ShowRouteCompass ? "On" : "Off");
    }
}