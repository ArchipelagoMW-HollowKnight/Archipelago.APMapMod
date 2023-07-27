using UnityEngine;
namespace ArchipelagoMapMod.UI;

internal class ShowReticleText : ControlPanelText
{
    private protected override string Name => "Show Reticle";

    private protected override bool ActiveCondition()
    {
        return ArchipelagoMapMod.GS.ControlPanelOn;
    }

    private protected override Vector4 GetColor()
    {
        return ArchipelagoMapMod.GS.ShowReticle
            ? APmmColors.GetColor(APmmColorSetting.UI_On)
            : APmmColors.GetColor(APmmColorSetting.UI_Neutral);
    }

    private protected override string GetText()
    {
        var text = "Show reticles (Ctrl-S): ";
        return text + (ArchipelagoMapMod.GS.ShowReticle ? "On" : "Off");
    }
}