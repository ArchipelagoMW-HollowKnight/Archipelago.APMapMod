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

        Button.BorderColor = apmmColors.GetColor(apmmColorSetting.UI_Borders);

        var text = "Vanilla:\n";

        if (ArchipelagoMapMod.LS.VanillaOn)
        {
            Button.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_On);
            text += "on";
        }
        else
        {
            Button.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_Neutral);
            text += "off";
        }

        if (IsVanillaCustom())
        {
            Button.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_Custom);
            text += " (custom)";
        }

        Button.Content = text;
    }

    internal static bool IsVanillaCustom()
    {
        if (ArchipelagoMapMod.LS.GroupBy == GroupBySetting.Item)
        {
            if (!apmmPinManager.VanillaItemPoolGroups.Any()) return false;

            return (!ArchipelagoMapMod.LS.VanillaOn && apmmPinManager.VanillaItemPoolGroups.Any(group =>
                       ArchipelagoMapMod.LS.GetPoolGroupSetting(group) == PoolState.On))
                   || (ArchipelagoMapMod.LS.VanillaOn && apmmPinManager.VanillaItemPoolGroups.Any(group =>
                       ArchipelagoMapMod.LS.GetPoolGroupSetting(group) == PoolState.Off));
        }

        if (!apmmPinManager.RandoLocationPoolGroups.Any()) return false;

        return (!ArchipelagoMapMod.LS.VanillaOn &&
                apmmPinManager.VanillaLocationPoolGroups.Any(group =>
                    ArchipelagoMapMod.LS.GetPoolGroupSetting(group) == PoolState.On))
               || (ArchipelagoMapMod.LS.VanillaOn && apmmPinManager.VanillaLocationPoolGroups.Any(group =>
                   ArchipelagoMapMod.LS.GetPoolGroupSetting(group) == PoolState.Off));
    }
}