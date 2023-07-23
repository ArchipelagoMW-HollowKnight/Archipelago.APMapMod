using MapChanger.UI;

namespace ArchipelagoMapMod.UI;

internal class PinOptionsPanel : ExtraButtonPanel
{
    private static readonly ExtraButton[] buttons =
    {
        new ClearedButton(),
        new PersistentButton(),
        new ReachablePinsButton()
    };

    public PinOptionsPanel() : base(nameof(PinOptionsPanel), ArchipelagoMapMod.MOD, 390f, 10)
    {
        Instance = this;
    }

    internal static PinOptionsPanel Instance { get; private set; }

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