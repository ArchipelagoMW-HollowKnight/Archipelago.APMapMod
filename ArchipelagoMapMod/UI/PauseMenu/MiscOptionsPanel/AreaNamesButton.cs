using MapChanger.UI;
using L = RandomizerMod.Localization;

namespace ArchipelagoMapMod.UI;

internal class AreaNamesButton : ExtraButton
{
    public AreaNamesButton() : base(nameof(AreaNamesButton), ArchipelagoMapMod.MOD)
    {
    }

    public override void Make()
    {
        base.Make();

        Button.Borderless = true;
    }

    protected override void OnClick()
    {
        ArchipelagoMapMod.GS.ToggleAreaNames();
    }

    protected override void OnHover()
    {
        apmmTitle.Instance.HoveredText = "Show area names on the world/quick map.";
    }

    protected override void OnUnhover()
    {
        apmmTitle.Instance.HoveredText = null;
    }

    public override void Update()
    {
        var text = $"{L.Localize("Show area\nnames")}: ";

        if (ArchipelagoMapMod.GS.ShowAreaNames)
        {
            text += L.Localize("On");
            Button.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_On);
        }
        else
        {
            text += L.Localize("Off");
            Button.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_Neutral);
        }

        Button.Content = text;
    }
}