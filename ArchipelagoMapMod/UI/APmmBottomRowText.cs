using ArchipelagoMapMod.Modes;
using ArchipelagoMapMod.Settings;
using MapChanger.UI;

namespace ArchipelagoMapMod.UI;

internal sealed class apmmBottomRowText : BottomRowText
{
    protected override float MinSpacing => 250f;

    protected override string[] TextNames => new[]
    {
        "Spoilers",
        "Randomized",
        "Vanilla",
        "Style",
        "Size"
    };

    protected override bool Condition()
    {
        return base.Condition() && Conditions.ArchipelagoMapModEnabled();
    }

    public override void Update()
    {
        UpdateSpoilers();
        UpdateRandomized();
        UpdateVanilla();
        UpdateStyle();
        UpdateSize();
    }

    private void UpdateSpoilers()
    {
        if (!MapTexts.TryGetValue("Spoilers", out var textObj)) return;

        var text = "Spoilers (ctrl-1): ";

        if (ArchipelagoMapMod.LS.SpoilerOn)
        {
            textObj.ContentColor = APmmColors.GetColor(APmmColorSetting.UI_On);
            text += "on";
        }
        else
        {
            textObj.ContentColor = APmmColors.GetColor(APmmColorSetting.UI_Neutral);
            text += "off";
        }

        textObj.Text = text;
    }

    private void UpdateRandomized()
    {
        if (!MapTexts.TryGetValue("Randomized", out var textObj)) return;

        var text = "Randomized (ctrl-2): ";

        if (ArchipelagoMapMod.LS.RandomizedOn)
        {
            textObj.ContentColor = APmmColors.GetColor(APmmColorSetting.UI_On);
            text += "on";
        }
        else
        {
            textObj.ContentColor = APmmColors.GetColor(APmmColorSetting.UI_Neutral);
            text += "off";
        }

        if (RandomizedButton.IsRandomizedCustom())
            textObj.ContentColor = APmmColors.GetColor(APmmColorSetting.UI_Custom);

        textObj.Text = text;
    }

    private void UpdateVanilla()
    {
        if (!MapTexts.TryGetValue("Vanilla", out var textObj)) return;

        var text = "Vanilla (ctrl-3): ";

        if (ArchipelagoMapMod.LS.VanillaOn)
        {
            textObj.ContentColor = APmmColors.GetColor(APmmColorSetting.UI_On);
            text += "on";
        }
        else
        {
            textObj.ContentColor = APmmColors.GetColor(APmmColorSetting.UI_Neutral);
            text += "off";
        }

        if (VanillaButton.IsVanillaCustom()) textObj.ContentColor = APmmColors.GetColor(APmmColorSetting.UI_Custom);

        textObj.Text = text;
    }

    private void UpdateStyle()
    {
        if (!MapTexts.TryGetValue("Style", out var textObj)) return;

        var text = "Style (ctrl-4): ";

        switch (ArchipelagoMapMod.GS.PinStyle)
        {
            case PinStyle.Normal:
                text += "normal";
                break;

            case PinStyle.Q_Marks_1:
                text += "q marks 1";
                break;

            case PinStyle.Q_Marks_2:
                text += "q marks 2";
                break;

            case PinStyle.Q_Marks_3:
                text += "q marks 3";
                break;
        }

        textObj.ContentColor = APmmColors.GetColor(APmmColorSetting.UI_Neutral);
        textObj.Text = text;
    }

    private void UpdateSize()
    {
        if (!MapTexts.TryGetValue("Size", out var textObj)) return;

        var text = "Size (ctrl-5): ";

        switch (ArchipelagoMapMod.GS.PinSize)
        {
            case PinSize.Small:
                text += "small";
                break;

            case PinSize.Medium:
                text += "medium";
                break;

            case PinSize.Large:
                text += "large";
                break;
        }

        textObj.ContentColor = APmmColors.GetColor(APmmColorSetting.UI_Neutral);
        textObj.Text = text;
    }
}