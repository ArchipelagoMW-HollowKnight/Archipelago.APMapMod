using ArchipelagoMapMod.Settings;
using MapChanger.UI;

namespace ArchipelagoMapMod.UI;

internal class PoolButton : ExtraButton
{
    public PoolButton(string poolGroup) : base(poolGroup, ArchipelagoMapMod.MOD)
    {
        PoolGroup = poolGroup;
    }

    internal string PoolGroup { get; init; }

    protected override void OnClick()
    {
        ArchipelagoMapMod.LS.TogglePoolGroupSetting(PoolGroup);
    }

    protected override void OnHover()
    {
        apmmTitle.Instance.HoveredText = $"Toggle {PoolGroup} on/off.";
    }

    protected override void OnUnhover()
    {
        apmmTitle.Instance.HoveredText = null;
    }

    public override void Update()
    {
        Button.Content = PoolGroup.Replace(" ", "\n");

        Button.ContentColor = ArchipelagoMapMod.LS.GetPoolGroupSetting(PoolGroup) switch
        {
            PoolState.On => apmmColors.GetColor(apmmColorSetting.UI_On),
            PoolState.Off => apmmColors.GetColor(apmmColorSetting.UI_Neutral),
            PoolState.Mixed => apmmColors.GetColor(apmmColorSetting.UI_Custom),
            _ => apmmColors.GetColor(apmmColorSetting.UI_On)
        };
    }
}