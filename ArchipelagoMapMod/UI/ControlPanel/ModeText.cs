using ArchipelagoMapMod.Modes;
using UnityEngine;

namespace ArchipelagoMapMod.UI;

internal class ModeText : ControlPanelText
{
    private protected override string Name => "Mode";

    private protected override bool ActiveCondition()
    {
        return ArchipelagoMapMod.GS.ControlPanelOn;
    }

    private protected override Vector4 GetColor()
    {
        if (MapChanger.Settings.CurrentMode() is FullMapMode)
            return APmmColors.GetColor(APmmColorSetting.UI_On);
        if (Conditions.TransitionRandoModeEnabled())
            return APmmColors.GetColor(APmmColorSetting.UI_Special);
        return APmmColors.GetColor(APmmColorSetting.UI_Neutral);
    }

    private protected override string GetText()
    {
        return $"Mode (Ctrl-T): {MapChanger.Settings.CurrentMode().ModeName}";
    }
}