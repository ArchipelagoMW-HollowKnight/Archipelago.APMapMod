using MapChanger.UI;
using L = RandomizerMod.Localization;

namespace ArchipelagoMapMod.UI;

internal class MapMarkersButton : ExtraButton
{
    public MapMarkersButton() : base(nameof(MapMarkersButton), ArchipelagoMapMod.MOD)
    {
    }

    public override void Make()
    {
        base.Make();

        Button.Borderless = true;
    }

    protected override void OnClick()
    {
        ArchipelagoMapMod.GS.ToggleMapMarkers();
    }

    protected override void OnHover()
    {
        apmmTitle.Instance.HoveredText = "Enable vanilla map marker behaviour.";
    }

    protected override void OnUnhover()
    {
        apmmTitle.Instance.HoveredText = null;
    }

    public override void Update()
    {
        var text = $"{L.Localize("Show map\nmarkers")}: ";

        if (ArchipelagoMapMod.GS.ShowMapMarkers)
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