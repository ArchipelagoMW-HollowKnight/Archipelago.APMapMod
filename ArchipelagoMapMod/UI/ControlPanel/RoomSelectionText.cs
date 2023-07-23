using ArchipelagoMapMod.Modes;
using UnityEngine;
using L = RandomizerMod.Localization;

namespace ArchipelagoMapMod.UI;

internal class RoomSelectionText : ControlPanelText
{
    private protected override string Name => "Room Selection";

    private protected override bool ActiveCondition()
    {
        return ArchipelagoMapMod.GS.ControlPanelOn && Conditions.TransitionRandoModeEnabled();
    }

    private protected override Vector4 GetColor()
    {
        return ArchipelagoMapMod.GS.RoomSelectionOn
            ? apmmColors.GetColor(apmmColorSetting.UI_On)
            : apmmColors.GetColor(apmmColorSetting.UI_Neutral);
    }

    private protected override string GetText()
    {
        var text = $"{L.Localize("Toggle room selection")} (Ctrl-R): ";
        return text + (ArchipelagoMapMod.GS.RoomSelectionOn ? "On" : "Off");
    }
}