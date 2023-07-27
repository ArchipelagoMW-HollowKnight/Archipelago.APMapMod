using ArchipelagoMapMod.Settings;
using MapChanger.UI;

namespace ArchipelagoMapMod.UI;

internal class PinSizeButton : MainButton
{
    internal PinSizeButton() : base(nameof(PinSizeButton), ArchipelagoMapMod.MOD, 1, 2)
    {
    }

    protected override void OnClick()
    {
        ArchipelagoMapMod.GS.TogglePinSize();
    }

    protected override void OnHover()
    {
        apmmTitle.Instance.HoveredText = "Toggle overall size of pins.";
    }

    protected override void OnUnhover()
    {
        apmmTitle.Instance.HoveredText = null;
    }

    public override void Update()
    {
        base.Update();

        Button.BorderColor = APmmColors.GetColor(APmmColorSetting.UI_Borders);

        var text = "Pin Size:\n";

        switch (ArchipelagoMapMod.GS.PinSize)
        {
            case PinSize.Small:
                text += "small";
                break;

            case PinSize.Medium:
                text += "medium";
                break;

            case PinSize.Large:
                text += "large";
                break;
        }

        Button.ContentColor = APmmColors.GetColor(APmmColorSetting.UI_Neutral);
        Button.Content = text;
    }
}