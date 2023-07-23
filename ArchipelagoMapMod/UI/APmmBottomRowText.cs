using ArchipelagoMapMod.Modes;
using ArchipelagoMapMod.Settings;
using MapChanger.UI;
using L = RandomizerMod.Localization;

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

        var text = $"{L.Localize("Spoilers")} (ctrl-1): ";

        if (ArchipelagoMapMod.LS.SpoilerOn)
        {
            textObj.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_On);
            text += L.Localize("on");
        }
        else
        {
            textObj.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_Neutral);
            text += L.Localize("off");
        }

        textObj.Text = text;
    }

    private void UpdateRandomized()
    {
        if (!MapTexts.TryGetValue("Randomized", out var textObj)) return;

        var text = $"{L.Localize("Randomized")} (ctrl-2): ";

        if (ArchipelagoMapMod.LS.RandomizedOn)
        {
            textObj.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_On);
            text += L.Localize("on");
        }
        else
        {
            textObj.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_Neutral);
            text += L.Localize("off");
        }

        if (RandomizedButton.IsRandomizedCustom())
            textObj.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_Custom);

        textObj.Text = text;
    }

    private void UpdateVanilla()
    {
        if (!MapTexts.TryGetValue("Vanilla", out var textObj)) return;

        var text = $"{L.Localize("Vanilla")} (ctrl-3): ";

        if (ArchipelagoMapMod.LS.VanillaOn)
        {
            textObj.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_On);
            text += L.Localize("on");
        }
        else
        {
            textObj.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_Neutral);
            text += L.Localize("off");
        }

        if (VanillaButton.IsVanillaCustom()) textObj.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_Custom);

        textObj.Text = text;
    }

    private void UpdateStyle()
    {
        if (!MapTexts.TryGetValue("Style", out var textObj)) return;

        var text = $"{L.Localize("Style")} (ctrl-4): ";

        switch (ArchipelagoMapMod.GS.PinStyle)
        {
            case PinStyle.Normal:
                text += L.Localize("normal");
                break;

            case PinStyle.Q_Marks_1:
                text += $"{L.Localize("q marks")} 1";
                break;

            case PinStyle.Q_Marks_2:
                text += $"{L.Localize("q marks")} 2";
                break;

            case PinStyle.Q_Marks_3:
                text += $"{L.Localize("q marks")} 3";
                break;
        }

        textObj.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_Neutral);
        textObj.Text = text;
    }

    private void UpdateSize()
    {
        if (!MapTexts.TryGetValue("Size", out var textObj)) return;

        var text = $"{L.Localize("Size")} (ctrl-5): ";

        switch (ArchipelagoMapMod.GS.PinSize)
        {
            case PinSize.Small:
                text += L.Localize("small");
                break;

            case PinSize.Medium:
                text += L.Localize("medium");
                break;

            case PinSize.Large:
                text += L.Localize("large");
                break;
        }

        textObj.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_Neutral);
        textObj.Text = text;
    }
}