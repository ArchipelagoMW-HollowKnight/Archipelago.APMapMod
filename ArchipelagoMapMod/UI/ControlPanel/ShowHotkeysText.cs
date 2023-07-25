using UnityEngine;

namespace ArchipelagoMapMod.UI;

internal class ShowHotkeysText : ControlPanelText
{
    private protected override string Name => "Show Hotkeys";

    private protected override bool ActiveCondition()
    {
        return true;
    }

    private protected override Vector4 GetColor()
    {
        return apmmColors.GetColor(apmmColorSetting.UI_Neutral);
    }

    private protected override string GetText()
    {
        if (ArchipelagoMapMod.GS.ControlPanelOn)
            return "Ctrl-H: Hide hotkeys";
        return "Ctrl-H: More hotkeys";
    }
}