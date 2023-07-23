using ArchipelagoMapMod.Modes;
using ArchipelagoMapMod.Pins;
using ArchipelagoMapMod.Rooms;
using MagicUI.Core;
using MagicUI.Elements;
using MagicUI.Graphics;
using MapChanger;
using MapChanger.MonoBehaviours;
using MapChanger.UI;

namespace ArchipelagoMapMod.UI;

internal class SelectionPanels : WorldMapStack
{
    private static Panel lookupPanel;
    private static TextObject pinPanelText;

    private static Panel roomPanel;
    private static TextObject roomPanelText;
    protected override HorizontalAlignment StackHorizontalAlignment => HorizontalAlignment.Right;

    protected override bool Condition()
    {
        return base.Condition() && Conditions.ArchipelagoMapModEnabled();
    }

    protected override void BuildStack()
    {
        lookupPanel = new Panel(Root,
            SpriteManager.Instance.GetTexture("GUI.PanelRight").ToSlicedSprite(100f, 50f, 200f, 50f), "Lookup Panel")
        {
            Borders = new Padding(30f, 30f, 30f, 30f),
            MinWidth = 200f,
            MinHeight = 100f,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center
        };

        ((Image) Root.GetElement("Lookup Panel Background")).Tint = apmmColors.GetColor(apmmColorSetting.UI_Borders);

        pinPanelText = new TextObject(Root, "Pin Panel Text")
        {
            ContentColor = apmmColors.GetColor(apmmColorSetting.UI_Neutral),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
            Font = MagicUI.Core.UI.Perpetua,
            FontSize = 20,
            MaxWidth = 450f
        };

        lookupPanel.Child = pinPanelText;

        Stack.Children.Add(lookupPanel);

        roomPanel = new Panel(Root,
            SpriteManager.Instance.GetTexture("GUI.PanelRight").ToSlicedSprite(100f, 50f, 250f, 50f), "Room Panel")
        {
            Borders = new Padding(30f, 30f, 30f, 30f),
            MinWidth = 200f,
            MinHeight = 100f,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center
        };

        ((Image) Root.GetElement("Room Panel Background")).Tint = apmmColors.GetColor(apmmColorSetting.UI_Borders);

        roomPanelText = new TextObject(Root, "Room Panel Text")
        {
            ContentColor = apmmColors.GetColor(apmmColorSetting.UI_Neutral),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
            Font = MagicUI.Core.UI.TrajanNormal,
            FontSize = 14,
            MaxWidth = 450f
        };

        roomPanel.Child = roomPanelText;

        Stack.Children.Add(roomPanel);
    }

    public override void Update()
    {
        UpdatePinPanel();
        UpdateRoomPanel();
    }

    internal static void UpdatePinPanel()
    {
        if (ArchipelagoMapMod.GS.PinSelectionOn &&
            apmmPinSelector.Instance.SelectedObjectKey is not Selector.NONE_SELECTED)
        {
            pinPanelText.Text = apmmPinSelector.Instance.GetText();
            lookupPanel.Visibility = Visibility.Visible;
        }
        else
        {
            lookupPanel.Visibility = Visibility.Collapsed;
        }
    }

    internal static void UpdateRoomPanel()
    {
        if (Conditions.TransitionRandoModeEnabled()
            && ArchipelagoMapMod.GS.RoomSelectionOn
            && TransitionRoomSelector.Instance.SelectedObjectKey is not Selector.NONE_SELECTED)
        {
            roomPanelText.Text = TransitionRoomSelector.Instance.GetText();
            roomPanel.Visibility = Visibility.Visible;
        }
        else
        {
            roomPanel.Visibility = Visibility.Collapsed;
        }
    }
}