using ArchipelagoMapMod.Settings;
using MapChanger.UI;

namespace ArchipelagoMapMod.UI;

internal class GroupByButton : ExtraButton
{
    internal GroupByButton() : base(nameof(GroupByButton), ArchipelagoMapMod.MOD)
    {
    }

    public override void Make()
    {
        base.Make();

        Button.Borderless = true;
    }

    protected override void OnClick()
    {
        ArchipelagoMapMod.LS.ToggleGroupBy();
    }

    protected override void OnHover()
    {
        apmmTitle.Instance.HoveredText = "Group pools by either location (normal) or by item (spoilers).";
    }

    protected override void OnUnhover()
    {
        apmmTitle.Instance.HoveredText = null;
    }

    public override void Update()
    {
        var text = "Group by:\n";

        switch (ArchipelagoMapMod.LS.GroupBy)
        {
            case GroupBySetting.Location:
                text += "Location";
                break;

            case GroupBySetting.Item:
                text += "Item";
                break;
        }

        Button.Content = text;
        Button.ContentColor = APmmColors.GetColor(APmmColorSetting.UI_Special);
    }
}