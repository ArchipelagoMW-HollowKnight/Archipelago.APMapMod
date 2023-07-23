using MapChanger.UI;

namespace ArchipelagoMapMod.UI;

internal class MiscOptionsPanel : ExtraButtonPanel
{
    private static readonly ExtraButton[] buttons =
    {
        new AreaNamesButton(),
        new NextAreasButton(),
        new MapMarkersButton(),
        new QuillButton(),
        new DefaultSettingsButton()
    };

    public MiscOptionsPanel() : base(nameof(MiscOptionsPanel), ArchipelagoMapMod.MOD, 390f, 10)
    {
        Instance = this;
    }

    internal static MiscOptionsPanel Instance { get; private set; }

    protected override void MakeButtons()
    {
        foreach (var button in buttons)
        {
            button.Make();
            ExtraButtonsGrid.Children.Add(button.Button);
            ExtraButtons.Add(button);
        }
    }
}