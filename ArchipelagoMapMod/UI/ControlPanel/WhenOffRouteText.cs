using ArchipelagoMapMod.Modes;
using ArchipelagoMapMod.Settings;
using UnityEngine;

namespace ArchipelagoMapMod.UI;

internal class WhenOffRouteText : ControlPanelText
{
    private protected override string Name => "When Off Route";

    private protected override bool ActiveCondition()
    {
        return ArchipelagoMapMod.GS.ControlPanelOn && Conditions.TransitionRandoModeEnabled();
    }

    private protected override Vector4 GetColor()
    {
        return ArchipelagoMapMod.GS.WhenOffRoute switch
        {
            OffRouteBehaviour.Reevaluate => APmmColors.GetColor(APmmColorSetting.UI_On),
            _ => APmmColors.GetColor(APmmColorSetting.UI_Neutral)
        };
    }

    private protected override string GetText()
    {
        var text = "When off-route (Ctrl-E): ";

        return ArchipelagoMapMod.GS.WhenOffRoute switch
        {
            OffRouteBehaviour.Keep => text + "Keep route",
            OffRouteBehaviour.Cancel => text + "Cancel route",
            OffRouteBehaviour.Reevaluate => text + "Reevaluate route",
            _ => text
        };
    }
}