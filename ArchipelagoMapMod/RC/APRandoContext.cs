using ArchipelagoMapMod.RandomizerData;
using ArchipelagoMapMod.Settings;
using Newtonsoft.Json;
using RandomizerCore;
using RandomizerCore.Logic;

namespace ArchipelagoMapMod.RC;

public class APRandoContext : RandoContext
{
    public APRandoContext(LogicManager LM) : base(LM) { }

    public APRandoContext(GenerationSettings gs, StartDef startDef) : base(RCData.GetNewLogicManager(gs))
    {
        base.InitialProgression = new ProgressionInitializer(LM, gs, startDef);
        this.GenerationSettings = gs;
        this.StartDef = startDef;
    }

    public APRandoContext(APRandoContext ctx) : base(ctx.LM)
    {
        notchCosts = ctx.notchCosts.ToList();
        itemPlacements = ctx.itemPlacements.ToList();
        transitionPlacements = ctx.transitionPlacements.ToList();
        StartDef = ctx.StartDef;
        InitialProgression = ctx.InitialProgression;
        Vanilla = ctx.Vanilla.ToList();
        GenerationSettings = ctx.GenerationSettings;
    }

    public GenerationSettings GenerationSettings { get; init; }
    public StartDef StartDef { get; init; }

    public List<GeneralizedPlacement> Vanilla = new();
    public List<ItemPlacement> itemPlacements = new();
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<TransitionPlacement> transitionPlacements = new();
    public List<int> notchCosts = new();

    public override IEnumerable<GeneralizedPlacement> EnumerateExistingPlacements()
    {
        foreach (GeneralizedPlacement p in Vanilla) yield return p;
        foreach (ItemPlacement p in itemPlacements) yield return p;
        foreach (TransitionPlacement p in transitionPlacements) yield return p;
    }

}