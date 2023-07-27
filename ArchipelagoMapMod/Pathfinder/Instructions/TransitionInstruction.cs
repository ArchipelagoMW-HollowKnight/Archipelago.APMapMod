using ArchipelagoMapMod.Transition;

namespace ArchipelagoMapMod.Pathfinder.Instructions;

internal class TransitionInstruction : Instruction
{
    internal TransitionInstruction(string name, string destination) : base(name, destination)
    {
        if (CompassObjects is null && TransitionData.GetTransitionDef(name) is APmmTransitionDef td)
            CompassObjects = new Dictionary<string, string> {{td.SceneName, td.DoorName}};
    }

    internal override bool IsFinished(ItemChanger.Transition lastTransition)
    {
        // Fix for big mantis village transition
        var lastTransitionFixed = lastTransition.ToString() switch
        {
            "Fungus2_15[top2]" => "Fungus2_15[top3]",
            "Fungus2_14[bot1]" => "Fungus2_14[bot3]",
            _ => lastTransition.ToString()
        };

        return TargetTransition == lastTransitionFixed;
    }
}