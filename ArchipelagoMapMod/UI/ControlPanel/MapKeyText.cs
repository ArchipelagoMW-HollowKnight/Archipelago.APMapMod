using UnityEngine;

namespace ArchipelagoMapMod.UI;

internal class MapKeyText : ControlPanelText
{
    private protected override string Name => "Map Key";

    private protected override bool ActiveCondition()
    {
        return ArchipelagoMapMod.GS.ControlPanelOn;
    }

    private protected override Vector4 GetColor()
    {
        return ArchipelagoMapMod.GS.MapKeyOn
            ? APmmColors.GetColor(APmmColorSetting.UI_On)
            : APmmColors.GetColor(APmmColorSetting.UI_Neutral);
    }

    private protected override string GetText()
    {
        var text = "Toggle map key (Ctrl-K): ";
        return text + (ArchipelagoMapMod.GS.MapKeyOn ? "On" : "Off");
    }
}