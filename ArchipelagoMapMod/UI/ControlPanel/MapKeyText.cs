using UnityEngine;
using L = RandomizerMod.Localization;

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
            ? apmmColors.GetColor(apmmColorSetting.UI_On)
            : apmmColors.GetColor(apmmColorSetting.UI_Neutral);
    }

    private protected override string GetText()
    {
        var text = $"{L.Localize("Toggle map key")} (Ctrl-K): ";
        return text + (ArchipelagoMapMod.GS.MapKeyOn ? "On" : "Off");
    }
}