using UnityEngine;
using L = RandomizerMod.Localization;

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
            ? apmmColors.GetColor(apmmColorSetting.UI_On)
            : apmmColors.GetColor(apmmColorSetting.UI_Neutral);
    }

    private protected override string GetText()
    {
        var text = $"{L.Localize("Toggle pin selection")} (Ctrl-P): ";
        return text + (ArchipelagoMapMod.GS.PinSelectionOn ? "On" : "Off");
    }
}