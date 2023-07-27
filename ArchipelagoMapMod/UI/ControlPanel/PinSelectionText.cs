using UnityEngine;

namespace ArchipelagoMapMod.UI;

internal class PinSelectionText : ControlPanelText
{
    private protected override string Name => "Pin Selection";

    private protected override bool ActiveCondition()
    {
        return ArchipelagoMapMod.GS.ControlPanelOn;
    }

    private protected override Vector4 GetColor()
    {
        return ArchipelagoMapMod.GS.PinSelectionOn
            ? APmmColors.GetColor(APmmColorSetting.UI_On)
            : APmmColors.GetColor(APmmColorSetting.UI_Neutral);
    }

    private protected override string GetText()
    {
        var text = "Toggle pin selection (Ctrl-P): ";
        return text + (ArchipelagoMapMod.GS.PinSelectionOn ? "On" : "Off");
    }
}