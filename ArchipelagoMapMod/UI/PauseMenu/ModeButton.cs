using ArchipelagoMapMod.Modes;
using MapChanger.UI;
using L = RandomizerMod.Localization;

namespace ArchipelagoMapMod.UI;

internal class ModeButton : MainButton
{
    public ModeButton() : base(nameof(ModeButton), ArchipelagoMapMod.MOD, 1, 0)
    {
    }

    protected override void OnClick()
    {
        MapChanger.Settings.ToggleMode();
    }

    protected override void OnHover()
    {
        apmmTitle.Instance.HoveredText = "Toggle to the next map mode.";
    }

    protected override void OnUnhover()
    {
        apmmTitle.Instance.HoveredText = null;
    }

    public override void Update()
    {
        base.Update();

        Button.BorderColor = apmmColors.GetColor(apmmColorSetting.UI_Borders);

        var text = $"{L.Localize("Mode")}:";

        var mode = MapChanger.Settings.CurrentMode();

        if (mode is FullMapMode)
        {
            Button.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_On);
            text += '\n' + L.Localize("Full Map");
        }

        if (mode is AllPinsMode)
        {
            Button.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_Neutral);
            text += '\n' + L.Localize("All Pins");
        }

        if (mode is PinsOverAreaMode)
        {
            Button.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_Neutral);
            text += L.Localize(" Pins\nOver Area");
        }

        if (mode is PinsOverRoomMode)
        {
            Button.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_Neutral);
            text += L.Localize(" Pins\nOver Room");
        }

        if (mode is TransitionNormalMode)
        {
            Button.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_Special);
            text += '\n' + L.Localize("Transition") + " 1";
        }

        if (mode is TransitionVisitedOnlyMode)
        {
            Button.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_Special);
            text += '\n' + L.Localize("Transition") + " 2";
        }

        if (mode is TransitionAllRoomsMode)
        {
            Button.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_Special);
            text += '\n' + L.Localize("Transition") + " 3";
        }

        Button.Content = text;
    }
}