using ArchipelagoMapMod.Modes;
using ArchipelagoMapMod.Settings;
using UnityEngine;
using L = RandomizerMod.Localization;

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
            OffRouteBehaviour.Reevaluate => apmmColors.GetColor(apmmColorSetting.UI_On),
            _ => apmmColors.GetColor(apmmColorSetting.UI_Neutral)
        };
    }

    private protected override string GetText()
    {
        var text = $"{L.Localize("When off-route")} (Ctrl-E): ";

        return ArchipelagoMapMod.GS.WhenOffRoute switch
        {
            OffRouteBehaviour.Keep => text + L.Localize("Keep route"),
            OffRouteBehaviour.Cancel => text + L.Localize("Cancel route"),
            OffRouteBehaviour.Reevaluate => text + L.Localize("Reevaluate route"),
            _ => text
        };
    }
}