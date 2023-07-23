using MapChanger.UI;
using L = RandomizerMod.Localization;

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
        var text = $"{L.Localize("Always have\nQuill")}: ";

        if (ArchipelagoMapMod.GS.AlwaysHaveQuill)
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