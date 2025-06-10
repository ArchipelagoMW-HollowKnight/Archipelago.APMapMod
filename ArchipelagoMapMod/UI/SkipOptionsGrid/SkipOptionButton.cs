using ArchipelagoMapMod.RC;
using ArchipelagoMapMod.Settings;
using MapChanger;
using MapChanger.UI;
using RandoMapCore;
using RandoMapCore.Data;
using RandoMapCore.UI;
using System.Reflection;

namespace ArchipelagoMapMod.UI.SkipOptionsGrid;

internal class SkipOptionButton(string skipName, FieldInfo field) : ExtraButton
{
    private APRandoContext Context => (APRandoContext)ApmmDataModule.Instance.Context;
    private SkipSettings Settings => Context.GenerationSettings.SkipSettings;
    private bool Value => (bool)field.GetValue(Settings);

    public override string Name => $"{Layout.Mod} {skipName}";

    protected override void OnClick()
    {
        field.SetValue(Settings, !Value);
        Context.InitialProgression = new ProgressionInitializer(
            Context.LM,
            Context.GenerationSettings,
            Context.GenerationSettings.StartDef
        );
        ApmmDataModule.Instance.ResetTracker();
        PlacementTracker.OnUpdate();
    }

    protected override TextFormat GetTextFormat()
    {
        return (
            skipName.L().Replace(" ", "\n"),
            Value ? RmcColorSetting.UI_On : RmcColorSetting.UI_Neutral
        ).ToTextFormat();
    }

    protected override TextFormat? GetHoverTextFormat()
    {
        return $"{"Toggle".L()} {skipName.L()} {"on/off".L()}.".ToNeutralTextFormat();
    }
}
