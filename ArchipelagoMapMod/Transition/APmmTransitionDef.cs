using System.Text.RegularExpressions;
using Newtonsoft.Json;
using ArchipelagoMapMod.RandomizerData;

namespace ArchipelagoMapMod.Transition;

public record APmmTransitionDef
{
    [JsonConstructor]
    public APmmTransitionDef(string sceneName, string doorName)
    {
        SceneName = sceneName;
        DoorName = doorName;
    }

    public APmmTransitionDef(TransitionDef td)
    {
        SceneName = td.SceneName;
        DoorName = td.DoorName;
    }

    public string Name => $"{SceneName}[{DoorName}]";
    public string SceneName { get; init; }
    public string DoorName { get; init; }

    public static bool TryMake(string str, out APmmTransitionDef td)
    {
        var match = Regex.Match(str, @"^(\w+)\[(\w+)\]$");

        if (match.Groups.Count == 3)
        {
            td = new APmmTransitionDef(match.Groups[1].Value, match.Groups[2].Value);
            return true;
        }

        td = default;
        return false;
    }
}