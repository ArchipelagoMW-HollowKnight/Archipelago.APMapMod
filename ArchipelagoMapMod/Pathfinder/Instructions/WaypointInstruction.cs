using Newtonsoft.Json;

namespace ArchipelagoMapMod.Pathfinder.Instructions;

internal class WaypointInstruction : Instruction
{
    [JsonConstructor]
    internal WaypointInstruction(string name, string targetTransition, string waypoint) : base(name, targetTransition)
    {
        Waypoint = waypoint;
    }

    internal string Waypoint { get; }
}