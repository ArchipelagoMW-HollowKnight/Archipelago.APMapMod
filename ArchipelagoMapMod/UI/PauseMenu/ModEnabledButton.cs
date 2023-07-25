using MagicUI.Core;
using MapChanger.UI;

namespace ArchipelagoMapMod.UI;

internal class ModEnabledButton : MainButton
{
    public ModEnabledButton() : base(nameof(ModEnabledButton), ArchipelagoMapMod.MOD, 0, 0)
    {
    }

    protected override void OnClick()
    {
        MapChanger.Settings.ToggleModEnabled();
    }

    protected override void OnHover()
    {
        apmmTitle.Instance.HoveredText = "Toggle all map mod behavior on/off.";
    }

    protected override void OnUnhover()
    {
        apmmTitle.Instance.HoveredText = null;
    }

    public override void Update()
    {
        Button.BorderColor = apmmColors.GetColor(apmmColorSetting.UI_Borders);

        if (MapChanger.Settings.CurrentMode().Mod is ArchipelagoMapMod.MOD)
            Button.Visibility = Visibility.Visible;
        else
            Button.Visibility = Visibility.Hidden;

        if (MapChanger.Settings.MapModEnabled())
        {
            Button.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_On);
            Button.Content = "Map Mod\nEnabled";
        }
        else
        {
            Button.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_Disabled);
            Button.Content = "Map Mod\nDisabled";
        }
    }
}