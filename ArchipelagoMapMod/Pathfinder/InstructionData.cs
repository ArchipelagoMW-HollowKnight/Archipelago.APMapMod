using System.Collections.ObjectModel;
using ArchipelagoMapMod.Pathfinder.Instructions;
using ArchipelagoMapMod.Transition;
using RandomizerMod.RandomizerData;
using RCPathfinder;
using RCPathfinder.Actions;
using JsonUtil = MapChanger.JsonUtil;

namespace ArchipelagoMapMod.Pathfinder;

internal class InstructionData
{
    internal static InstructionData Instance;

    private static readonly HashSet<string> extraRooms = new()
    {
        "Room_Final_Boss_Atrium",
        "GG_Atrium",
        "GG_Workshop"
    };

    internal InstructionData(apmmSearchData sd)
    {
        Instance = this;

        Dictionary<string, TransitionInstruction> transitionInstructions = new();

        foreach (var action in sd.Actions)
            if (action.IsOrIsSubclassInstanceOf<PlacementAction>())
                transitionInstructions[action.Name] = new TransitionInstruction(action.Name, action.Destination.Name);

        Instructions = new ReadOnlyCollection<Instruction>(transitionInstructions.Values.ToArray());
        TransitionInstructions = new ReadOnlyDictionary<string, TransitionInstruction>(transitionInstructions);

        if (Interop.HasBenchwarp())
        {
            StartWarpInstruction = new StartWarpInstruction(sd.StartTerm);
            BenchwarpInstructions = new ReadOnlyDictionary<string, BenchwarpInstruction>(
                BenchwarpInterop.BenchKeys.ToDictionary(b => b.Key,
                    b => new BenchwarpInstruction(b.Key, b.Value, b.Key)));
        }
    }

    internal static ReadOnlyDictionary<(string waypoint, string targetScene), WaypointInstruction> WaypointInstructions
    {
        get;
        private set;
    }

    internal ReadOnlyCollection<Instruction> Instructions { get; }
    internal ReadOnlyDictionary<string, TransitionInstruction> TransitionInstructions { get; }
    internal StartWarpInstruction StartWarpInstruction { get; }
    internal ReadOnlyDictionary<string, BenchwarpInstruction> BenchwarpInstructions { get; }
    internal DreamgateInstruction DreamgateInstruction { get; private set; }

    //private readonly EmptyInstruction empty = new();

    //private readonly Dictionary<(string, string), MiscTransitionInstruction> miscTransitionInstructions = new();
    //private readonly Dictionary<(string, string), MiscInstruction> miscInstructions = new();

    internal static void LoadWaypointInstructions()
    {
        var waypointInstructions = JsonUtil.DeserializeFromAssembly<WaypointInstruction[]>(ArchipelagoMapMod.Assembly,
            "ArchipelagoMapMod.Resources.Pathfinder.Data.waypointInstructions.json");

        Dictionary<(string, string), WaypointInstruction> wiLookup = new();

        foreach (var wi in waypointInstructions) wiLookup[(wi.Waypoint, wi.TargetScene)] = wi;

        WaypointInstructions =
            new ReadOnlyDictionary<(string waypoint, string targetScene), WaypointInstruction>(wiLookup);
    }

    internal static List<Instruction> GetInstructions(Node node)
    {
        List<Instruction> instructions = new();

        var position = node.StartPosition.Name;
        TransitionData.TryGetScene(position, out var scene);

        if (Interop.HasBenchwarp())
        {
            if (position == Instance.StartWarpInstruction.Waypoint)
            {
                instructions.Add(Instance.StartWarpInstruction);
                scene = Instance.StartWarpInstruction.TargetScene;
            }

            if (Instance.BenchwarpInstructions.TryGetValue(position, out var benchWarp))
            {
                instructions.Add(benchWarp);
                scene = benchWarp.TargetScene;
            }
        }

        if (node.Key is "dreamGate") instructions.Add(Instance.DreamgateInstruction);

        foreach (var action in node.Actions)
        {
            if (TryGetInstruction(position, scene, action, out var instruction))
            {
                instructions.Add(instruction);
                scene = instruction.TargetScene;

                //if (instruction == Instance.empty) return new();
            }

            position = action.Destination.Name;
        }

        return instructions;
    }

    private static bool TryGetInstruction(string position, string scene, AbstractAction action,
        out Instruction instruction)
    {
        if (Instance is null) throw new NullReferenceException(nameof(Instance));

        if (action.IsOrIsSubclassInstanceOf<PlacementAction>())
        {
            instruction = Instance.TransitionInstructions[action.Name];
            return true;
        }

        // We need to get the waypoint instruction that matches the current scene, current position and target scene.
        // The scene gets propagated from the previous term if current position doesn't have a scene.
        // This ensures that the scene/newScene comparison is correct.
        if (action.IsOrIsSubclassInstanceOf<StateLogicAction>())
        {
            string newScene;
            if (extraRooms.Contains(action.Destination.Name) || Data.IsRoom(action.Destination.Name))
            {
                newScene = action.Destination.Name;
            }
            else if (!TransitionData.TryGetScene(action.Destination.Name, out newScene))
            {
                instruction = default;
                return false;
            }

            if (scene != newScene)
                if (WaypointInstructions.TryGetValue((position, newScene), out var wi))
                {
                    instruction = wi;
                    return true;
                }
            //// Fallback handling for when the position is a transition and its target is in the newScene - assume going through the transition
            //else if (TransitionData.Placements.TryGetValue(action.Name, out string target)
            //    && TransitionData.GetTransitionDef(target) is apmmTransitionDef targetTd
            //    && targetTd.SceneName == newScene)
            //{
            //    ArchipelagoMapMod.Instance?.LogDebug($"New best guess TransitionInstruction: {action.Name}-?>{target}-?>{action.Destination.Name}");
            //    instruction = Instance.TransitionInstructions[action.Name];
            //}
            //// The destination is a transition
            //else if (TransitionData.GetTransitionDef(action.Destination.Name) is not null)
            //{
            //    ArchipelagoMapMod.Instance?.LogDebug($"New MiscTranstisionInstruction: {scene}-?>{action.Destination.Name}");
            //    instruction = GetOrAddMiscTransitionInstruction(scene, action.Destination.Name);
            //}
            //else
            //{
            //    ArchipelagoMapMod.Instance?.LogDebug($"New MiscInstruction: {scene}-?>{newScene}");
            //    instruction = GetOrAddMiscInstruction(scene, newScene);
            //}
            //else
            //{
            //    //ArchipelagoMapMod.Instance?.LogDebug($"Empty instruction for action {action.Name}, {position}, {scene}, {newScene}, {action.Destination.Name}");
            //    instruction = Instance.empty;
            //}
            //return true;
            //ArchipelagoMapMod.Instance?.LogDebug($"No instruction for action {action.Name}, {position}, {scene}, {newScene}, {action.Destination.Name}");
        }

        instruction = default;
        return false;
    }

    internal static void UpdateDreamgateInstruction(string dreamgateTiedTransition)
    {
        Instance.DreamgateInstruction = new DreamgateInstruction(dreamgateTiedTransition);
    }

    //private static MiscTransitionInstruction GetOrAddMiscTransitionInstruction(string scene, string destination)
    //{
    //    if (Instance.miscTransitionInstructions.TryGetValue((scene, destination), out var instruction))
    //    {
    //        return instruction;
    //    }

    //    return Instance.miscTransitionInstructions[(scene, destination)] = new MiscTransitionInstruction(scene, destination);
    //}

    //private static MiscInstruction GetOrAddMiscInstruction(string scene, string newScene)
    //{
    //    if (Instance.miscInstructions.TryGetValue((scene, newScene), out var instruction))
    //    {
    //        return instruction;
    //    }

    //    return Instance.miscInstructions[(scene, newScene)] = new MiscInstruction(scene, newScene);
    //}
}