using UnityEngine;
using L = RandomizerMod.Localization;

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
                ? apmmColors.GetColor(apmmColorSetting.UI_On)
                : apmmColors.GetColor(apmmColorSetting.UI_Neutral);

        return apmmColors.GetColor(apmmColorSetting.UI_Neutral);
    }

    private protected override string GetText()
    {
        if (Interop.HasBenchwarp())
        {
            var text = $"{L.Localize("Benchwarp pins")} (Ctrl-W): ";
            return text + (ArchipelagoMapMod.GS.ShowBenchwarpPins ? "On" : "Off");
        }

        return "Benchwarp is not installed or outdated";
    }
}