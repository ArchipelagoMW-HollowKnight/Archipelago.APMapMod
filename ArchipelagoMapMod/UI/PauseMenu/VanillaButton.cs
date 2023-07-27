using ArchipelagoMapMod.Pins;
using ArchipelagoMapMod.Settings;
using MapChanger.UI;

namespace ArchipelagoMapMod.UI;

internal class VanillaButton : MainButton
{
    internal VanillaButton() : base(nameof(VanillaButton), ArchipelagoMapMod.MOD, 0, 2)
    {
    }

    protected override void OnClick()
    {
        ArchipelagoMapMod.LS.ToggleVanilla();
    }

    protected override void OnHover()
    {
        apmmTitle.Instance.HoveredText = "Toggle pins for vanilla locations on/off.";
    }

    protected override void OnUnhover()
    {
        apmmTitle.Instance.HoveredText = null;
    }

    public override void Update()
    {
        base.Update();

        Button.BorderColor = APmmColors.GetColor(APmmColorSetting.UI_Borders);

        var text = "Vanilla:\n";

        if (ArchipelagoMapMod.LS.VanillaOn)
        {
            Button.ContentColor = APmmColors.GetColor(APmmColorSetting.UI_On);
            text += "on";
        }
        else
        {
            Button.ContentColor = APmmColors.GetColor(APmmColorSetting.UI_Neutral);
            text += "off";
        }

        if (IsVanillaCustom())
        {
            Button.ContentColor = APmmColors.GetColor(APmmColorSetting.UI_Custom);
            text += " (custom)";
        }

        Button.Content = text;
    }

    internal static bool IsVanillaCustom()
    {
        if (ArchipelagoMapMod.LS.GroupBy == GroupBySetting.Item)
        {
            if (!APmmPinManager.VanillaItemPoolGroups.Any()) return false;

            return (!ArchipelagoMapMod.LS.VanillaOn && APmmPinManager.VanillaItemPoolGroups.Any(group =>
                       ArchipelagoMapMod.LS.GetPoolGroupSetting(group) == PoolState.On))
                   || (ArchipelagoMapMod.LS.VanillaOn && APmmPinManager.VanillaItemPoolGroups.Any(group =>
                       ArchipelagoMapMod.LS.GetPoolGroupSetting(group) == PoolState.Off));
        }

        if (!APmmPinManager.RandoLocationPoolGroups.Any()) return false;

        return (!ArchipelagoMapMod.LS.VanillaOn &&
                APmmPinManager.VanillaLocationPoolGroups.Any(group =>
                    ArchipelagoMapMod.LS.GetPoolGroupSetting(group) == PoolState.On))
               || (ArchipelagoMapMod.LS.VanillaOn && APmmPinManager.VanillaLocationPoolGroups.Any(group =>
                   ArchipelagoMapMod.LS.GetPoolGroupSetting(group) == PoolState.Off));
    }
}