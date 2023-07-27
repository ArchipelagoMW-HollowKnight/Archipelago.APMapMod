using UnityEngine;

namespace ArchipelagoMapMod.UI;

internal class BenchwarpPinsText : ControlPanelText
{
    private protected override string Name => "Benchwarp Pins";

    private protected override bool ActiveCondition()
    {
        return ArchipelagoMapMod.GS.ControlPanelOn;
    }

    private protected override Vector4 GetColor()
    {
        if (Interop.HasBenchwarp())
            return ArchipelagoMapMod.GS.ShowBenchwarpPins
                ? APmmColors.GetColor(APmmColorSetting.UI_On)
                : APmmColors.GetColor(APmmColorSetting.UI_Neutral);

        return APmmColors.GetColor(APmmColorSetting.UI_Neutral);
    }

    private protected override string GetText()
    {
        if (Interop.HasBenchwarp())
        {
            var text = "Benchwarp pins (Ctrl-W): ";
            return text + (ArchipelagoMapMod.GS.ShowBenchwarpPins ? "On" : "Off");
        }

        return "Benchwarp is not installed or outdated";
    }
}