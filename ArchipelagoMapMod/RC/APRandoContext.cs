using Archipelago.HollowKnight.IC;
using ArchipelagoMapMod.IC;
using ArchipelagoMapMod.RandomizerData;
using ArchipelagoMapMod.Settings;
using ConnectionMetadataInjector;
using ItemChanger.Internal;
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

        Vanilla = new List<GeneralizedPlacement>();
        
        foreach (var transition in Data.Transitions)
        {
            if (transition.Value.VanillaTarget == null) continue;
#if DEBUG
            ArchipelagoMapMod.Instance.LogDebug(
                $"creating transition {transition.Key} and linking it to {transition.Value.VanillaTarget}");
#endif
            var item = LM.TransitionLookup[transition.Key];
            var location = LM.GetTransition(transition.Value.VanillaTarget);
            Vanilla.Add(new GeneralizedPlacement(item, location));
        }

        var externalItems = ItemChanger.Finder.ItemNames.ToList(); 
        itemPlacements = new List<ItemPlacement>();
        foreach (var entry in Ref.Settings.Placements)
        {
            entry.Value.GetOrAddTag<APmmPlacementTag>();
            
            foreach (var abstractItem in entry.Value.Items)
            {
                APItem item = new(abstractItem.name);

                APLocation location = new(LM.GetLogicDef(entry.Key));

                //check if this is an AP item.
                if (abstractItem.GetTag(out ArchipelagoItemTag aptag))
                {
                    if ((bool) Util.Get(GenerationSettings,
                            $"PoolSettings.{SupplementalMetadata.Of(entry.Value).Get(InjectedProps.LocationPoolGroup).Replace(" ", "")}"))
                    {
                        abstractItem.GetOrAddTag<APmmItemTag>();
                        
                        // only add an actual logical item if its for us. otherwise leave it as a dummy item
                        if (aptag.Player == Archipelago.HollowKnight.Archipelago.Instance.session.ConnectionInfo.Slot)
                        {
                            item.item = LM.GetItem(abstractItem.name);
                            // item is for us we can remove it from the external list
                            externalItems.Remove(abstractItem.name);
                        }

                        ArchipelagoMapMod.Instance.LogDebug(
                                $"Creating Item Placement [{aptag.Player}] {item.item?.Name} at {entry.Key}");
                            itemPlacements.Add(new ItemPlacement(item, location));
                            
                    }
                    else
                    {
                        ArchipelagoMapMod.Instance.LogDebug(
                            $"Creating Vanilla Item Placement [{aptag.Player}] {item.item?.Name} at {entry.Key}");
                        Vanilla.Add(new ItemPlacement(item, location));
                    }
                }
            }
            
        }
        
        ArchipelagoMapMod.Instance.LogDebug($"Local Items set, {externalItems.Count} remain adding to start region.");
        foreach (var externalItem in externalItems)
        {
            // there are some oddball items like the charm repair and Iselda's map pins that are not in here
            // so just skip those.
            if (! LM.ItemLookup.ContainsKey(externalItem)) continue;
            
            APItem item = new(LM.GetItem(externalItem));
            APLocation location = new(LM.GetLogicDef("Start"));

            itemPlacements.Add(new ItemPlacement(item, location));
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