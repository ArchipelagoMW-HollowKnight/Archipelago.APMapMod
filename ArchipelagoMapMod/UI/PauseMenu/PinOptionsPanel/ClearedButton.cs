using MapChanger.UI;

namespace ArchipelagoMapMod.UI;

internal class ClearedButton : ExtraButton
{
    internal ClearedButton() : base(nameof(ClearedButton), ArchipelagoMapMod.MOD)
    {
    }

    public override void Make()
    {
        base.Make();

        Button.Borderless = true;
    }

    protected override void OnClick()
    {
        ArchipelagoMapMod.GS.ToggleCleared();
    }

    protected override void OnHover()
    {
        apmmTitle.Instance.HoveredText = "Forces cleared locations to always show.";
    }

    protected override void OnUnhover()
    {
        apmmTitle.Instance.HoveredText = null;
    }

    public override void Update()
    {
        var text = "Cleared\nlocations: ";

        if (ArchipelagoMapMod.GS.ShowClearedPins)
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