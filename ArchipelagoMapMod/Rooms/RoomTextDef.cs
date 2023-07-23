using GlobalEnums;
using MapChanger.Defs;
using Newtonsoft.Json;

namespace ArchipelagoMapMod.Rooms;

internal record RoomTextDef : IMapPosition
{
    [JsonProperty] public string Name { get; init; }

    [JsonProperty] public MapZone MapZone { get; init; }

    [JsonProperty] public float X { get; set; }

    [JsonProperty] public float Y { get; set; }
}