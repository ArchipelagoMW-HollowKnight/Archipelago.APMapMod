using MagicUI.Core;
using MagicUI.Elements;

namespace ArchipelagoMapMod.UI;

internal static class UIExtensions
{
    // TODO: move some of these into MapChanger
    internal static TextObject TextFromEdge(LayoutRoot onLayout, string name, bool onRight)
    {
        var text = BaseText(onLayout, name);

        if (onRight)
        {
            text.HorizontalAlignment = HorizontalAlignment.Right;
            text.TextAlignment = HorizontalAlignment.Right;
            text.Padding = new Padding(0f, 20f, 20f, 0f);
        }
        else
        {
            text.Padding = new Padding(20f, 20f, 0f, 0f);
        }

        return text;
    }

    internal static TextObject PanelText(LayoutRoot onLayout, string name)
    {
        var text = BaseText(onLayout, name);
        text.VerticalAlignment = VerticalAlignment.Center;
        text.Padding = new Padding(0f, 2f, 0f, 2f);
        return text;
    }

    private static TextObject BaseText(LayoutRoot onLayout, string name)
    {
        return new TextObject(onLayout, name)
        {
            ContentColor = apmmColors.GetColor(apmmColorSetting.UI_Neutral),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            TextAlignment = HorizontalAlignment.Left,
            Font = MagicUI.Core.UI.TrajanNormal,
            FontSize = 14
        };
    }

    internal static void SetToggleText(TextObject textObj, string baseText, bool value)
    {
        var text = baseText;

        if (value)
        {
            textObj.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_On);
            text += "On";
        }
        else
        {
            textObj.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_Neutral);
            text += "Off";
        }

        textObj.Text = text;
    }
}