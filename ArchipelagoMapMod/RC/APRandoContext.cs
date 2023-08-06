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

        //List<string> externalItems = ItemChanger.Finder.ItemNames.ToList(); 
        itemPlacements = new List<ItemPlacement>();
        foreach (KeyValuePair<string, AbstractPlacement> entry in Ref.Settings.Placements)
        {
            
            foreach (AbstractItem abstractItem in entry.Value.Items)
            {
                APItem item = new(abstractItem.name);

                APLocation location = new(LM.GetLogicDef(entry.Key));

                //check if this is an AP item.
                if (abstractItem.GetTag(out ArchipelagoItemTag aptag))
                {
                    if ((bool) GenerationSettings.Get($"PoolSettings.{SupplementalMetadata.Of(entry.Value).Get(InjectedProps.LocationPoolGroup).Replace(" ", "")}"))
                    {
                        entry.Value.GetOrAddTag<APmmPlacementTag>();
                        abstractItem.GetOrAddTag<APmmItemTag>();
                        
                        // only add an actual logical item if its for us. otherwise leave it as a dummy item
                        if (aptag.Player == Archipelago.HollowKnight.Archipelago.Instance.session.ConnectionInfo.Slot)
                        {
                            item.item = LM.GetItem(abstractItem.name);
                        }
                        itemPlacements.Add(new ItemPlacement(item, location));
                    }
                    else
                    {
                        Vanilla.Add(new ItemPlacement(item, location));
                    }
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