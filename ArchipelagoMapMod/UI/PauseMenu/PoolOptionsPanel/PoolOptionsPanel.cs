using ArchipelagoMapMod.Pins;
using MapChanger.UI;

namespace ArchipelagoMapMod.UI;

internal class PoolOptionsPanel : ExtraButtonPanel
{
    public PoolOptionsPanel() : base(nameof(PoolOptionsPanel), ArchipelagoMapMod.MOD, 390f, 10)
    {
        Instance = this;
    }

    internal static PoolOptionsPanel Instance { get; private set; }

    protected override void MakeButtons()
    {
        foreach (var poolGroup in APmmPinManager.AllPoolGroups)
        {
            PoolButton poolButton = new(poolGroup);
            poolButton.Make();
            ExtraButtonsGrid.Children.Add(poolButton.Button);
            ExtraButtons.Add(poolButton);
        }

        GroupByButton groupByButton = new();
        groupByButton.Make();
        ExtraButtonsGrid.Children.Add(groupByButton.Button);
        ExtraButtons.Add(groupByButton);
    }
}