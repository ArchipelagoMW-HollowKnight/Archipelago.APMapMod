using ArchipelagoMapMod.Settings;
using MapChanger.UI;

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
        var text = "Show next\nareas: ";

        switch (ArchipelagoMapMod.GS.ShowNextAreas)
        {
            case NextAreaSetting.Off:
                text += "Off";
                Button.ContentColor = APmmColors.GetColor(APmmColorSetting.UI_Neutral);
                break;

            case NextAreaSetting.Arrows:
                text += "Arrows";
                Button.ContentColor = APmmColors.GetColor(APmmColorSetting.UI_On);
                break;

            case NextAreaSetting.Full:
                text += "Full";
                Button.ContentColor = APmmColors.GetColor(APmmColorSetting.UI_On);
                break;
        }

        Button.Content = text;
    }
}