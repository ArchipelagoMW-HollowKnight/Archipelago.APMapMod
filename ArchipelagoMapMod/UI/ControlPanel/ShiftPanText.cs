using UnityEngine;
namespace ArchipelagoMapMod.UI;

internal class ShiftPanText : ControlPanelText
{
    private protected override string Name => "Shift Pan";

    private protected override bool ActiveCondition()
    {
        return ArchipelagoMapMod.GS.ControlPanelOn;
    }

    private protected override Vector4 GetColor()
    {
        return APmmColors.GetColor(APmmColorSetting.UI_Neutral);
    }

    private protected override string GetText()
    {
        return "Hold Shift: Pan faster";
    }
}