using ArchipelagoMapMod.Modes;
using UnityEngine;

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
            ? APmmColors.GetColor(APmmColorSetting.UI_On)
            : APmmColors.GetColor(APmmColorSetting.UI_Neutral);
    }

    private protected override string GetText()
    {
        var text = "Toggle room selection (Ctrl-R): ";
        return text + (ArchipelagoMapMod.GS.RoomSelectionOn ? "On" : "Off");
    }
}