using ArchipelagoMapMod.Settings;
using MapChanger.UI;
using L = RandomizerMod.Localization;

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

        Button.BorderColor = apmmColors.GetColor(apmmColorSetting.UI_Borders);

        var text = $"{L.Localize("Pin Size")}:\n";

        switch (ArchipelagoMapMod.GS.PinSize)
        {
            case PinSize.Small:
                text += L.Localize("small");
                break;

            case PinSize.Medium:
                text += L.Localize("medium");
                break;

            case PinSize.Large:
                text += L.Localize("large");
                break;
        }

        Button.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_Neutral);
        Button.Content = text;
    }
}