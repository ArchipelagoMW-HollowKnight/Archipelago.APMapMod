using ArchipelagoMapMod.Settings;
using MapChanger.UI;
using L = RandomizerMod.Localization;

namespace ArchipelagoMapMod.UI;

internal class NextAreasButton : ExtraButton
{
    public NextAreasButton() : base(nameof(NextAreasButton), ArchipelagoMapMod.MOD)
    {
    }

    public override void Make()
    {
        base.Make();

        Button.Borderless = true;
    }

    protected override void OnClick()
    {
        ArchipelagoMapMod.GS.ToggleNextAreas();
    }

    protected override void OnHover()
    {
        apmmTitle.Instance.HoveredText = "Show next area indicators (text/arrow) on the quick map.";
    }

    protected override void OnUnhover()
    {
        apmmTitle.Instance.HoveredText = null;
    }

    public override void Update()
    {
        var text = $"{L.Localize("Show next\nareas")}: ";

        switch (ArchipelagoMapMod.GS.ShowNextAreas)
        {
            case NextAreaSetting.Off:
                text += L.Localize("Off");
                Button.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_Neutral);
                break;

            case NextAreaSetting.Arrows:
                text += L.Localize("Arrows");
                Button.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_On);
                break;

            case NextAreaSetting.Full:
                text += L.Localize("Full");
                Button.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_On);
                break;
        }

        Button.Content = text;
    }
}