using MapChanger.UI;

namespace ArchipelagoMapMod.UI;

internal class PersistentButton : ExtraButton
{
    public PersistentButton() : base(nameof(PersistentButton), ArchipelagoMapMod.MOD)
    {
    }

    public override void Make()
    {
        base.Make();

        Button.Borderless = true;
    }

    protected override void OnClick()
    {
        ArchipelagoMapMod.GS.TogglePersistent();
    }

    protected override void OnHover()
    {
        apmmTitle.Instance.HoveredText = "Forces persistent items to always show.";
    }

    protected override void OnUnhover()
    {
        apmmTitle.Instance.HoveredText = null;
    }

    public override void Update()
    {
        var text = "Persistent\nitems: ";

        if (ArchipelagoMapMod.GS.ShowPersistentPins)
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