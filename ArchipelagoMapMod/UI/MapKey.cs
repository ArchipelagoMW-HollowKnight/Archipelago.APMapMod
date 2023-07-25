using ArchipelagoMapMod.Modes;
using MagicUI.Core;
using MagicUI.Elements;
using MagicUI.Graphics;
using MapChanger;
using MapChanger.UI;
using UnityEngine;

namespace ArchipelagoMapMod.UI;

internal class MapKey : WorldMapStack
{
    private static Panel panel;
    private static StackLayout panelContents;
    private static GridLayout pinKey;
    private static GridLayout roomKey;

    protected override void BuildStack()
    {
        panel = new Panel(Root, SpriteManager.Instance.GetTexture("GUI.PanelLeft").ToSlicedSprite(200f, 50f, 100f, 50f),
            "Panel")
        {
            MinHeight = 0f,
            MinWidth = 0f,
            Borders = new Padding(0f, 20f, 20f, 20f),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top
        };

        ((Image) Root.GetElement("Panel Background")).Tint = apmmColors.GetColor(apmmColorSetting.UI_Borders);

        Stack.Children.Add(panel);

        panelContents = new StackLayout(Root, "Panel Contents")
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Orientation = Orientation.Horizontal,
            Spacing = 5f
        };

        panel.Child = panelContents;

        pinKey = new GridLayout(Root, "Pin Key")
        {
            MinWidth = 200f,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Top,
            RowDefinitions =
            {
                new GridDimension(1, GridUnit.Proportional),
                new GridDimension(1, GridUnit.Proportional),
                new GridDimension(1, GridUnit.Proportional),
                new GridDimension(1, GridUnit.Proportional),
                new GridDimension(1, GridUnit.Proportional)
            },
            ColumnDefinitions =
            {
                new GridDimension(1, GridUnit.Proportional),
                new GridDimension(1.6f, GridUnit.Proportional)
            }
        };

        panelContents.Children.Add(pinKey);

        var counter = 0;

        foreach (var colorSetting in apmmColors.PinColors)
        {
            var pinPanel = new Panel(Root, SpriteManager.Instance.GetSprite("Pins.Blank"), colorSetting + "Panel")
            {
                MinHeight = 50f,
                MinWidth = 50f,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }.WithProp(GridLayout.Column, 0).WithProp(GridLayout.Row, counter);

            var pin = new Image(Root, SpriteManager.Instance.GetSprite("Pins.Border"), colorSetting + " Pin")
            {
                Width = 50f,
                Height = 50f,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }.WithProp(GridLayout.Column, 0).WithProp(GridLayout.Row, counter);

            ((Image) Root.GetElement(colorSetting + " Pin")).Tint = apmmColors.GetColor(colorSetting);

            pinPanel.Child = pin;

            var text = new TextObject(Root, colorSetting + " Text")
            {
                Text = colorSetting.ToString().Replace("Pin_", "".ToCleanName()),
                Padding = new Padding(10f, 0f, 0f, 0f),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            }.WithProp(GridLayout.Column, 1).WithProp(GridLayout.Row, counter);

            pinKey.Children.Add(pinPanel);
            pinKey.Children.Add(text);

            counter++;
        }

        roomKey = new GridLayout(Root, "Room Key")
        {
            MinWidth = 200f,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            RowDefinitions =
            {
                new GridDimension(1, GridUnit.Proportional),
                new GridDimension(1, GridUnit.Proportional),
                new GridDimension(1, GridUnit.Proportional),
                new GridDimension(1, GridUnit.Proportional),
                new GridDimension(1, GridUnit.Proportional),
                new GridDimension(1, GridUnit.Proportional)
            },
            ColumnDefinitions =
            {
                new GridDimension(1, GridUnit.Proportional),
                new GridDimension(1.6f, GridUnit.Proportional)
            }
        };

        panelContents.Children.Add(roomKey);

        var roomCopy = GameManager.instance.gameMap.transform.GetChild(12).transform.GetChild(26)
            .GetComponent<SpriteRenderer>().sprite;

        counter = 0;

        foreach (var color in apmmColors.RoomColors)
        {
            var cleanRoomColor = color.ToString().Replace("Room_", "").ToCleanName();

            var room = new Image(Root, roomCopy, cleanRoomColor + " Room")
            {
                Width = 40f,
                Height = 40f,
                Tint = apmmColors.GetColor(color),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Padding = new Padding(0f, 5f, 17f, 5f)
            }.WithProp(GridLayout.Column, 0).WithProp(GridLayout.Row, counter);

            var text = new TextObject(Root, cleanRoomColor + " Text")
            {
                Text = cleanRoomColor,
                Padding = new Padding(10f, 0f, 0f, 0f),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            }.WithProp(GridLayout.Column, 1).WithProp(GridLayout.Row, counter);

            roomKey.Children.Add(room);
            roomKey.Children.Add(text);

            counter++;
        }

        var highlighted = apmmColors.GetColor(apmmColorSetting.Room_Normal);
        highlighted.w = 1f;

        var roomHighlight = new Image(Root, roomCopy, "Highlighted Room")
        {
            Width = 40f,
            Height = 40f,
            Tint = highlighted,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
            Padding = new Padding(0f, 5f, 17f, 5f)
        }.WithProp(GridLayout.Column, 0).WithProp(GridLayout.Row, counter);

        var textHighlight = new TextObject(Root, "Highlighted Text")
        {
            Text = "Contains\nunchecked\ntransitions",
            Padding = new Padding(10f, 0f, 0f, 0f),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center
        }.WithProp(GridLayout.Column, 1).WithProp(GridLayout.Row, counter);

        roomKey.Children.Add(roomHighlight);
        roomKey.Children.Add(textHighlight);
    }

    protected override bool Condition()
    {
        return base.Condition() && Conditions.ArchipelagoMapModEnabled();
    }

    public override void Update()
    {
        if (ArchipelagoMapMod.GS.MapKeyOn)
            panel.Visibility = Visibility.Visible;
        else
            panel.Visibility = Visibility.Hidden;

        if (Conditions.TransitionRandoModeEnabled())
            roomKey.Visibility = Visibility.Visible;
        else
            roomKey.Visibility = Visibility.Collapsed;
    }
}