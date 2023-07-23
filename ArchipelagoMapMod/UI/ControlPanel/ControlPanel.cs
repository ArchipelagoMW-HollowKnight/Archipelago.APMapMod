using ArchipelagoMapMod.Modes;
using MagicUI.Core;
using MagicUI.Elements;
using MagicUI.Graphics;
using MapChanger;
using MapChanger.UI;

namespace ArchipelagoMapMod.UI;

internal class ControlPanel : WorldMapStack
{
    private static Panel panel;
    private static StackLayout panelStack;

    private static readonly ControlPanelText[] texts =
    {
        new ShowHotkeysText(),
        new ModEnabledText(),
        new ModeText(),
        new ShiftPanText(),
        new MapKeyText(),
        new PinSelectionText(),
        new BenchwarpPinsText(),
        new RoomSelectionText(),
        new ShowReticleText(),
        new PathfinderBenchwarpText(),
        new ShowRouteInGameText(),
        new WhenOffRouteText(),
        new CompassText()
    };

    protected override HorizontalAlignment StackHorizontalAlignment => HorizontalAlignment.Left;
    protected override VerticalAlignment StackVerticalAlignment => VerticalAlignment.Bottom;

    protected override void BuildStack()
    {
        panel = new Panel(Root, SpriteManager.Instance.GetTexture("GUI.PanelLeft").ToSlicedSprite(200f, 50f, 100f, 50f),
            "Panel")
        {
            MinWidth = 0f,
            MinHeight = 0f,
            Borders = new Padding(10f, 20f, 30f, 20f),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Bottom
        };

        Stack.Children.Add(panel);

        ((Image) Root.GetElement("Panel Background")).Tint = apmmColors.GetColor(apmmColorSetting.UI_Borders);

        panelStack = new StackLayout(Root, "Panel Stack")
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Orientation = Orientation.Vertical
        };

        panel.Child = panelStack;

        foreach (var cpt in texts) cpt.Make(Root, panelStack);
    }

    protected override bool Condition()
    {
        return base.Condition() && Conditions.ArchipelagoMapModEnabled();
    }

    public override void Update()
    {
        foreach (var cpt in texts) cpt.Update();
    }
}