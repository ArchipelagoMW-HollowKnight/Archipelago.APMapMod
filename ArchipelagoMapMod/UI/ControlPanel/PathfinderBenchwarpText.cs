using ArchipelagoMapMod.Modes;
using UnityEngine;
using L = RandomizerMod.Localization;

namespace ArchipelagoMapMod.UI;

internal class PathfinderBenchwarpText : ControlPanelText
{
    private protected override string Name => "Pathfinder Benchwarp";

    private protected override bool ActiveCondition()
    {
        return ArchipelagoMapMod.GS.ControlPanelOn && Conditions.TransitionRandoModeEnabled();
    }

    private protected override Vector4 GetColor()
    {
        if (Interop.HasBenchwarp())
            return ArchipelagoMapMod.GS.PathfinderBenchwarp
                ? apmmColors.GetColor(apmmColorSetting.UI_On)
                : apmmColors.GetColor(apmmColorSetting.UI_Neutral);

        return apmmColors.GetColor(apmmColorSetting.UI_Neutral);
    }

    private protected override string GetText()
    {
        if (Interop.HasBenchwarp())
        {
            var text = $"{L.Localize("Pathfinder benchwarp")} (Ctrl-B): ";
            return text + (ArchipelagoMapMod.GS.PathfinderBenchwarp ? "On" : "Off");
        }

        return "Benchwarp is not installed or outdated";
    }
}