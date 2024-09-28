using Archipelago.HollowKnight;
using MapChanger.UI;

namespace ArchipelagoMapMod.UI;

internal class SpoilersButton : MainButton
{
    internal SpoilersButton() : base(nameof(SpoilersButton), ArchipelagoMapMod.MOD, 0, 3)
    {
    }

    protected override void OnClick()
    {
        ArchipelagoMapMod.LS.ToggleSpoilers();
    }

    protected override void OnHover()
    {
        apmmTitle.Instance.HoveredText = "Reveals the items at each location.";
    }

    protected override void OnUnhover()
    {
        apmmTitle.Instance.HoveredText = null;
    }

    public override void Update()
    {
        base.Update();

        Button.BorderColor = APmmColors.GetColor(APmmColorSetting.UI_Borders);

        if (ArchipelagoMapMod.LS.SpoilerOn && !ArchipelagoMod.Instance.SlotData.DisableLocalSpoilerLogs)
        {
            Button.ContentColor = APmmColors.GetColor(APmmColorSetting.UI_On);
            Button.Content = "Spoilers:\non";
        }
        else
        {
            Button.ContentColor = APmmColors.GetColor(APmmColorSetting.UI_Neutral);
            Button.Content = "Spoilers:\noff";
        }
    }
}