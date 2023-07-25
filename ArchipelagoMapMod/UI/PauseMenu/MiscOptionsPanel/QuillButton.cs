using MapChanger.UI;

namespace ArchipelagoMapMod.UI;

internal class QuillButton : ExtraButton
{
    public QuillButton() : base(nameof(QuillButton), ArchipelagoMapMod.MOD)
    {
    }

    public override void Make()
    {
        base.Make();

        Button.Borderless = true;
    }

    protected override void OnClick()
    {
        ArchipelagoMapMod.GS.ToggleAlwaysHaveQuill();
    }

    protected override void OnHover()
    {
        apmmTitle.Instance.HoveredText = "Doesn't affect Full Map and Transition modes.";
    }

    protected override void OnUnhover()
    {
        apmmTitle.Instance.HoveredText = null;
    }

    public override void Update()
    {
        var text = "Always have\nQuill: ";

        if (ArchipelagoMapMod.GS.AlwaysHaveQuill)
        {
            text += "On";
            Button.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_On);
        }
        else
        {
            text += "Off";
            Button.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_Neutral);
        }

        Button.Content = text;
    }
}