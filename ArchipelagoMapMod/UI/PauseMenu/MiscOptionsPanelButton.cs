using MagicUI.Core;
using MapChanger.UI;

namespace ArchipelagoMapMod.UI;

internal class MiscOptionsPanelButton : MainButton
{
    public MiscOptionsPanelButton() : base(nameof(MiscOptionsPanelButton), ArchipelagoMapMod.MOD, 2, 1)
    {
        Instance = this;
    }

    internal static MiscOptionsPanelButton Instance { get; private set; }

    protected override void OnClick()
    {
        MiscOptionsPanel.Instance.Toggle();
    }

    protected override void OnHover()
    {
        apmmTitle.Instance.HoveredText = "Some miscenalleous options.";
    }

    protected override void OnUnhover()
    {
        apmmTitle.Instance.HoveredText = null;
    }

    public override void Update()
    {
        base.Update();

        Button.BorderColor = APmmColors.GetColor(APmmColorSetting.UI_Borders);

        if (MiscOptionsPanel.Instance.ExtraButtonsGrid.Visibility == Visibility.Visible)
            Button.ContentColor = APmmColors.GetColor(APmmColorSetting.UI_Custom);
        else
            Button.ContentColor = APmmColors.GetColor(APmmColorSetting.UI_Neutral);

        Button.Content = "Misc.\nOptions";
    }
}