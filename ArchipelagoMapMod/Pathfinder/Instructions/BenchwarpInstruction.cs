namespace ArchipelagoMapMod.Pathfinder.Instructions;

internal class BenchwarpInstruction : WaypointInstruction
{
    internal BenchwarpInstruction(string name, apmmBenchKey benchKey, string waypoint)
        : base(name, $"{benchKey.SceneName}[]", waypoint)
    {
        BenchKey = benchKey;
    }

    internal apmmBenchKey BenchKey { get; }

    internal override bool IsFinished(ItemChanger.Transition lastTransition)
    {
        return lastTransition.ToString() == TargetTransition
               && PlayerData.instance.GetString("respawnMarkerName") == BenchKey.RespawnMarkerName;
    }
}