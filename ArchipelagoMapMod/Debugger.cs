using MapChanger;
using MapChanger.Defs;

namespace ArchipelagoMapMod;

internal static class Debugger
{
    internal static void LogMapPosition()
    {
        WorldMapPosition wmp = new(new MapLocation[]
        {
            new()
            {
                MappedScene = Utils.CurrentScene(),
                X = HeroController.instance.transform.position.x,
                Y = HeroController.instance.transform.position.y
            }
        });

        ArchipelagoMapMod.Instance.LogDebug(
            $"Absolute offset on map, snapped to 0.1: {Math.Round(wmp.X, 1)}, {Math.Round(wmp.Y, 1)}");
        ArchipelagoMapMod.Instance.LogDebug(
            $"Relative offset from center of map room, snapped to 0.1: {Math.Round(wmp.RelativeX, 1)}, {Math.Round(wmp.RelativeY, 1)}");
    }
}