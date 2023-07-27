using MagicUI.Core;
using MapChanger.UI;

namespace ArchipelagoMapMod.UI;

internal class PinOptionsPanelButton : MainButton
{
    public PinOptionsPanelButton() : base(nameof(PinOptionsPanelButton), ArchipelagoMapMod.MOD, 2, 0)
    {
        Instance = this;
    }

    internal static PinOptionsPanelButton Instance { get; private set; }

    protected override void OnClick()
    {
        PinOptionsPanel.Instance.Toggle();
    }

    protected override void OnHover()
    {
        apmmTitle.Instance.HoveredText = "More options for pin behavior.";
    }

    protected override void OnUnhover()
    {
        apmmTitle.Instance.HoveredText = null;
    }

    public override void Update()
    {
        base.Update();

        Button.BorderColor = APmmColors.GetColor(APmmColorSetting.UI_Borders);

        if (PinOptionsPanel.Instance.ExtraButtonsGrid.Visibility == Visibility.Visible)
            Button.ContentColor = APmmColors.GetColor(APmmColorSetting.UI_Custom);
        else
            Button.ContentColor = APmmColors.GetColor(APmmColorSetting.UI_Neutral);

        Button.Content = "More Pin\nOptions";
    }
}