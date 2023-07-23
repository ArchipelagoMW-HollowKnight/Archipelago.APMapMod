using ArchipelagoMapMod.Settings;
using MapChanger.UI;
using L = RandomizerMod.Localization;

namespace ArchipelagoMapMod.UI;

internal class DefaultSettingsButton : ExtraButton
{
    public DefaultSettingsButton() : base(nameof(DefaultSettingsButton), ArchipelagoMapMod.MOD)
    {
    }

    public override void Make()
    {
        base.Make();

        Button.Borderless = true;
    }

    protected override void OnClick()
    {
        GlobalSettings.ResetToDefaultSettings();
    }

    protected override void OnHover()
    {
        apmmTitle.Instance.HoveredText = "Resets all global settings of ArchipelagoMapMod.";
    }

    protected override void OnUnhover()
    {
        apmmTitle.Instance.HoveredText = null;
    }

    public override void Update()
    {
        var text = $"{L.Localize("Reset global\nsettings")}";

        Button.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_Special);

        Button.Content = text;
    }
}