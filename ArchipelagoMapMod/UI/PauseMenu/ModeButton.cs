using ArchipelagoMapMod.Modes;
using MapChanger.UI;

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

        Button.BorderColor = APmmColors.GetColor(APmmColorSetting.UI_Borders);

        var text = "Mode:";

        var mode = MapChanger.Settings.CurrentMode();

        if (mode is FullMapMode)
        {
            Button.ContentColor = APmmColors.GetColor(APmmColorSetting.UI_On);
            text += '\n' + "Full Map";
        }

        if (mode is AllPinsMode)
        {
            Button.ContentColor = APmmColors.GetColor(APmmColorSetting.UI_Neutral);
            text += '\n' + "All Pins";
        }

        if (mode is PinsOverAreaMode)
        {
            Button.ContentColor = APmmColors.GetColor(APmmColorSetting.UI_Neutral);
            text += " Pins\nOver Area";
        }

        if (mode is PinsOverRoomMode)
        {
            Button.ContentColor = APmmColors.GetColor(APmmColorSetting.UI_Neutral);
            text += " Pins\nOver Room";
        }

        if (mode is TransitionNormalMode)
        {
            Button.ContentColor = APmmColors.GetColor(APmmColorSetting.UI_Special);
            text += '\n' + "Transition 1";
        }

        if (mode is TransitionVisitedOnlyMode)
        {
            Button.ContentColor = APmmColors.GetColor(APmmColorSetting.UI_Special);
            text += '\n' + "Transition 2";
        }

        if (mode is TransitionAllRoomsMode)
        {
            Button.ContentColor = APmmColors.GetColor(APmmColorSetting.UI_Special);
            text += '\n' + "Transition 3";
        }

        Button.Content = text;
    }
}