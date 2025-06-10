using MapChanger;
using MapChanger.UI;
using RandoMapCore;
using RandoMapCore.UI;

namespace ArchipelagoMapMod.UI.SkipOptionsGrid;
internal class SkipOptionsControlButton : RmcGridControlButton<SkipOptionsGrid>
{
    protected override TextFormat GetTextFormat()
    {
        return (
            "Customize\nSkips".L(),
            GridOpen() ? RmcColorSetting.UI_Custom : RmcColorSetting.UI_Neutral
        ).ToTextFormat();
    }

    protected override TextFormat? GetHoverTextFormat()
    {
        return "Customize which skip settings are enabled in logic.".L().ToNeutralTextFormat();
    }
}
