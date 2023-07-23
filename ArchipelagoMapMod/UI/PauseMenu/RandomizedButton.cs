using ArchipelagoMapMod.Pins;
using ArchipelagoMapMod.Settings;
using MapChanger.UI;
using L = RandomizerMod.Localization;

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

        Button.BorderColor = apmmColors.GetColor(apmmColorSetting.UI_Borders);

        var text = $"{L.Localize("Randomized")}:\n";

        if (ArchipelagoMapMod.LS.RandomizedOn)
        {
            Button.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_On);
            text += L.Localize("on");
        }
        else
        {
            Button.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_Neutral);
            text += L.Localize("off");
        }

        if (IsRandomizedCustom())
        {
            Button.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_Custom);
            text += $" ({L.Localize("custom")})";
        }

        Button.Content = text;
    }

    internal static bool IsRandomizedCustom()
    {
        if (ArchipelagoMapMod.LS.GroupBy == GroupBySetting.Item)
        {
            if (!apmmPinManager.RandoItemPoolGroups.Any()) return false;

            return (!ArchipelagoMapMod.LS.RandomizedOn && apmmPinManager.RandoItemPoolGroups.Any(group =>
                       ArchipelagoMapMod.LS.GetPoolGroupSetting(group) == PoolState.On))
                   || (ArchipelagoMapMod.LS.RandomizedOn && apmmPinManager.RandoItemPoolGroups.Any(group =>
                       ArchipelagoMapMod.LS.GetPoolGroupSetting(group) == PoolState.Off));
        }

        if (!apmmPinManager.RandoLocationPoolGroups.Any()) return false;

        return (!ArchipelagoMapMod.LS.RandomizedOn &&
                apmmPinManager.RandoLocationPoolGroups.Any(group =>
                    ArchipelagoMapMod.LS.GetPoolGroupSetting(group) == PoolState.On))
               || (ArchipelagoMapMod.LS.RandomizedOn && apmmPinManager.RandoLocationPoolGroups.Any(group =>
                   ArchipelagoMapMod.LS.GetPoolGroupSetting(group) == PoolState.Off));
    }
}