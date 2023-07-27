using ArchipelagoMapMod.Modes;
using UnityEngine;

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
                ? APmmColors.GetColor(APmmColorSetting.UI_On)
                : APmmColors.GetColor(APmmColorSetting.UI_Neutral);

        return APmmColors.GetColor(APmmColorSetting.UI_Neutral);
    }

    private protected override string GetText()
    {
        if (Interop.HasBenchwarp())
        {
            var text = $"Pathfinder benchwarp (Ctrl-B): ";
            return text + (ArchipelagoMapMod.GS.PathfinderBenchwarp ? "On" : "Off");
        }

        return "Benchwarp is not installed or outdated";
    }
}