using Archipelago.HollowKnight.IC;
using ArchipelagoMapMod.IC;
using ArchipelagoMapMod.RandomizerData;
using ArchipelagoMapMod.Settings;
using ConnectionMetadataInjector;
using ItemChanger;
using ItemChanger.Internal;
using Newtonsoft.Json;
using RandomizerCore;
using RandomizerCore.Logic;
using StartDef = ArchipelagoMapMod.RandomizerData.StartDef;

namespace ArchipelagoMapMod.RC;

public class APRandoContext : RandoContext
{
    public APRandoContext(LogicManager LM) : base(LM) { }

    public APRandoContext(GenerationSettings gs, StartDef startDef) : base(RCData.GetNewLogicManager(gs))
    {
        base.InitialProgression = new ProgressionInitializer(LM, gs, startDef);
        this.GenerationSettings = gs;
        this.StartDef = startDef;

        Vanilla = new List<GeneralizedPlacement>();
        
        foreach (KeyValuePair<string, TransitionDef> transition in Data.Transitions)
        {
            if (transition.Value.VanillaTarget == null) continue;
            LogicTransition location = LM.TransitionLookup[transition.Key];
            LogicTransition item = LM.GetTransition(transition.Value.VanillaTarget);
            Vanilla.Add(new GeneralizedPlacement(item, location));
        }
        
        itemPlacements = new List<ItemPlacement>();
        foreach (AbstractPlacement placement in Ref.Settings.Placements.Values)
        {
            foreach (AbstractItem item in placement.Items)
            {
                //check if this is an AP item.
                if (!item.GetTag(out ArchipelagoItemTag aptag)) continue;
                
                

                APLocation logicLocation = new(LM.GetLogicDef(placement.Name));
                    
                if ((bool) GenerationSettings.Get($"PoolSettings.{SupplementalMetadata.Of(placement).Get(InjectedProps.LocationPoolGroup).Replace(" ", "")}"))
                {
                    placement.GetOrAddTag<APmmPlacementTag>();
                    APmmItemTag itemTag = item.GetOrAddTag<APmmItemTag>();

                    APItem logicItem;
                    
                    // create new APItem and attach a logic item to it if its logically relevant
                    if (aptag.IsItemForMe)
                    {
                        logicItem = new APItem(item.name)
                        {
                            item = LM.GetItem(item.name)
                        };
                        ArchipelagoMapMod.Instance.LogDebug($"[AP RC] Adding Randomized self-item {item.name} to logic context at {placement.Name}.");
                    }
                    else
                    {
                        // create an EmptyItem that has no logic terms attached to it.
                        logicItem = new APItem(item.name + "_player_" + aptag.Player);
                        ArchipelagoMapMod.Instance.LogDebug($"[AP RC] Adding Randomized other-item {item.name} to logic context as {logicItem.Name} at {placement.Name}.");
                    }

                    itemTag.id = itemPlacements.Count;
                    itemPlacements.Add(new ItemPlacement(logicItem, logicLocation));
                }
                else
                {
                    
                    Vanilla.Add(new ItemPlacement(new APItem(item.name), logicLocation));
                }
            }
        }
        for (int i = 0; i < itemPlacements.Count; i++)
        {
            itemPlacements[i] = itemPlacements[i] with { Index = i };
        }
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