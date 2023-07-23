namespace ArchipelagoMapMod.Modes;

internal static class Conditions
{
    internal static bool ArchipelagoMapModEnabled()
    {
        return MapChanger.Settings.MapModEnabled() &&
               MapChanger.Settings.CurrentMode().GetType().IsSubclassOf(typeof(apmmMapMode));
    }

    internal static bool ItemRandoModeEnabled()
    {
        return MapChanger.Settings.MapModEnabled() &&
               MapChanger.Settings.CurrentMode().GetType().IsSubclassOf(typeof(ItemRandoMode));
    }

    internal static bool TransitionRandoModeEnabled()
    {
        return MapChanger.Settings.MapModEnabled() &&
               MapChanger.Settings.CurrentMode().GetType().IsSubclassOf(typeof(TransitionRandoMode));
    }
}