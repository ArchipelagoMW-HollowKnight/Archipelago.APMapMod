using MagicUI.Core;
using MapChanger.UI;

namespace ArchipelagoMapMod.UI;

internal class PoolOptionsPanelButton : MainButton
{
    public PoolOptionsPanelButton() : base(nameof(PoolOptionsPanelButton), ArchipelagoMapMod.MOD, 1, 3)
    {
        Instance = this;
    }

    internal static PoolOptionsPanelButton Instance { get; private set; }

    protected override void OnClick()
    {
        PoolOptionsPanel.Instance.Toggle();
    }

    protected override void OnHover()
    {
        apmmTitle.Instance.HoveredText = "Customize which item/location pools to display.";
    }

    protected override void OnUnhover()
    {
        apmmTitle.Instance.HoveredText = null;
    }

    public override void Update()
    {
        base.Update();

        Button.BorderColor = apmmColors.GetColor(apmmColorSetting.UI_Borders);

        if (PoolOptionsPanel.Instance.ExtraButtonsGrid.Visibility == Visibility.Visible)
            Button.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_Custom);
        else
            Button.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_Neutral);

        Button.Content = "Customize\nPools";
    }
}