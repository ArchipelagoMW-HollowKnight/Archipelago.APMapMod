using ArchipelagoMapMod.Settings;
using MapChanger.UI;

namespace ArchipelagoMapMod.UI;

internal class PinStyleButton : MainButton
{
    internal PinStyleButton() : base(nameof(PinStyleButton), ArchipelagoMapMod.MOD, 1, 1)
    {
    }

    protected override void OnClick()
    {
        ArchipelagoMapMod.GS.TogglePinStyle();
    }

    protected override void OnHover()
    {
        apmmTitle.Instance.HoveredText = "Toggle the sprites used per pool.";
    }

    protected override void OnUnhover()
    {
        apmmTitle.Instance.HoveredText = null;
    }

    public override void Update()
    {
        base.Update();

        Button.BorderColor = apmmColors.GetColor(apmmColorSetting.UI_Borders);

        var text = "Pin Style:\n";

        switch (ArchipelagoMapMod.GS.PinStyle)
        {
            case PinStyle.Normal:
                text += "normal";
                break;

            case PinStyle.Q_Marks_1:
                text += "q marks 1";
                break;

            case PinStyle.Q_Marks_2:
                text += "q marks 2";
                break;

            case PinStyle.Q_Marks_3:
                text += "q marks 3";
                break;
        }

        Button.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_Neutral);
        Button.Content = text;
    }
}