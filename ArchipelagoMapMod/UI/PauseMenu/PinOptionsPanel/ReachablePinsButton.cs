using MapChanger.UI;

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
        var text = "Indicate\nreachable: ";

        if (ArchipelagoMapMod.GS.ReachablePins)
        {
            text += "On";
            Button.ContentColor = APmmColors.GetColor(APmmColorSetting.UI_On);
        }
        else
        {
            text += "Off";
            Button.ContentColor = APmmColors.GetColor(APmmColorSetting.UI_Neutral);
        }

        Button.Content = text;
    }
}