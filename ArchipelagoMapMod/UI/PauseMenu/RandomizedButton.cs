using ArchipelagoMapMod.Pins;
using ArchipelagoMapMod.Settings;
using MapChanger.UI;

namespace ArchipelagoMapMod.UI;

internal class RandomizedButton : MainButton
{
    internal RandomizedButton() : base(nameof(RandomizedButton), ArchipelagoMapMod.MOD, 0, 1)
    {
    }

    protected override void OnClick()
    {
        ArchipelagoMapMod.LS.ToggleRandomized();
    }

    protected override void OnHover()
    {
        apmmTitle.Instance.HoveredText = "Toggle pins for randomized locations on/off.";
    }

    protected override void OnUnhover()
    {
        apmmTitle.Instance.HoveredText = null;
    }

    public override void Update()
    {
        base.Update();

        Button.BorderColor = APmmColors.GetColor(APmmColorSetting.UI_Borders);

        var text = "Randomized:\n";

        if (ArchipelagoMapMod.LS.RandomizedOn)
        {
            Button.ContentColor = APmmColors.GetColor(APmmColorSetting.UI_On);
            text += "on";
        }
        else
        {
            Button.ContentColor = APmmColors.GetColor(APmmColorSetting.UI_Neutral);
            text += "off";
        }

        if (IsRandomizedCustom())
        {
            Button.ContentColor = APmmColors.GetColor(APmmColorSetting.UI_Custom);
            text += " (custom)";
        }

        Button.Content = text;
    }

    internal static bool IsRandomizedCustom()
    {
        if (ArchipelagoMapMod.LS.GroupBy == GroupBySetting.Item)
        {
            if (!APmmPinManager.RandoItemPoolGroups.Any()) return false;

            return (!ArchipelagoMapMod.LS.RandomizedOn && APmmPinManager.RandoItemPoolGroups.Any(group =>
                       ArchipelagoMapMod.LS.GetPoolGroupSetting(group) == PoolState.On))
                   || (ArchipelagoMapMod.LS.RandomizedOn && APmmPinManager.RandoItemPoolGroups.Any(group =>
                       ArchipelagoMapMod.LS.GetPoolGroupSetting(group) == PoolState.Off));
        }

        if (!APmmPinManager.RandoLocationPoolGroups.Any()) return false;

        return (!ArchipelagoMapMod.LS.RandomizedOn &&
                APmmPinManager.RandoLocationPoolGroups.Any(group =>
                    ArchipelagoMapMod.LS.GetPoolGroupSetting(group) == PoolState.On))
               || (ArchipelagoMapMod.LS.RandomizedOn && APmmPinManager.RandoLocationPoolGroups.Any(group =>
                   ArchipelagoMapMod.LS.GetPoolGroupSetting(group) == PoolState.Off));
    }
}