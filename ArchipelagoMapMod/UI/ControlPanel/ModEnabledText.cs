using UnityEngine;

namespace ArchipelagoMapMod.UI;

internal class ModEnabledText : ControlPanelText
{
    private protected override string Name => "Mod Enabled";

    private protected override bool ActiveCondition()
    {
        return ArchipelagoMapMod.GS.ControlPanelOn;
    }

    private protected override Vector4 GetColor()
    {
        return apmmColors.GetColor(apmmColorSetting.UI_Neutral);
    }

    private protected override string GetText()
    {
        return "Ctrl-M: Disable mod";
    }
}