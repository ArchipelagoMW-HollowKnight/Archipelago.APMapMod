using MapChanger.UI;
using L = RandomizerMod.Localization;

namespace ArchipelagoMapMod.UI;

internal class ReachablePinsButton : ExtraButton
{
    public ReachablePinsButton() : base(nameof(ReachablePinsButton), ArchipelagoMapMod.MOD)
    {
    }

    public override void Make()
    {
        base.Make();

        Button.Borderless = true;
    }

    protected override void OnClick()
    {
        ArchipelagoMapMod.GS.ToggleReachablePins();
    }

    protected override void OnHover()
    {
        apmmTitle.Instance.HoveredText = "Pins for unreachable locations are smaller/grayed out.";
    }

    protected override void OnUnhover()
    {
        apmmTitle.Instance.HoveredText = null;
    }

    public override void Update()
    {
        var text = $"{L.Localize("Indicate\nreachable")}: ";

        if (ArchipelagoMapMod.GS.ReachablePins)
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