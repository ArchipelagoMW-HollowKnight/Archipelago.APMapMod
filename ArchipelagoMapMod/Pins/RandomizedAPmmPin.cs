using Archipelago.HollowKnight.IC;
using ArchipelagoMapMod.Rooms;
using ArchipelagoMapMod.Settings;
using ConnectionMetadataInjector;
using ItemChanger;
using MapChanger;
using MapChanger.Defs;
using MapChanger.MonoBehaviours;
using System.Collections;
using UnityEngine;
using Finder = ItemChanger.Finder;
using RPS = ArchipelagoMapMod.Pins.RandoPlacementState;
using SD = ConnectionMetadataInjector.SupplementalMetadata;

namespace ArchipelagoMapMod.Pins;

internal sealed class RandomizedAPmmPin : APmmPin, IPeriodicUpdater
{
    private int itemIndex;

    private Dictionary<AbstractItem, string> itemPoolGroups;
    private Dictionary<AbstractItem, (ISprite, float)> itemSprites;

    private ISprite locationSprite;
    private float locationSpriteScale;

    private Coroutine periodicUpdate;
    private AbstractPlacement placement;
    internal RPS placementState;

    private IEnumerable<AbstractItem> remainingItems;

    private bool showItemSprite;
    internal override HashSet<string> ItemPoolGroups => new(itemPoolGroups.Values);

    internal string[] HighlightScenes { get; private set; }
    internal HashSet<ISelectable> HighlightRooms { get; private set; }

    public float UpdateWaitSeconds { get; } = 1f;

    public IEnumerator PeriodicUpdate()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(UpdateWaitSeconds);
            itemIndex = (itemIndex + 1) % remainingItems.Count();
            UpdatePinSprites();
        }
    }

    private void StartPeriodicUpdate()
    {
        if (periodicUpdate is null)
        {
            periodicUpdate = StartCoroutine(PeriodicUpdate());
        }
    }

    private void StopPeriodicUpdate()
    {
        if (periodicUpdate is not null)
        {
            StopCoroutine(periodicUpdate);
            periodicUpdate = null;
        }
    }

    internal void Initialize(AbstractPlacement placement)
    {
        Initialize();

        this.placement = placement;

        SceneName = placement.RandoModLocation()?.LocationDef?.SceneName ?? Finder.GetLocation(name)?.sceneName;

        ModSource = SD.Of(placement).Get(InteropProperties.ModSource);

        LocationPoolGroup = SD.Of(placement).Get(InjectedProps.LocationPoolGroup);
        if (SD.Of(placement).Get(InteropProperties.LocationPinSpriteKey) is string locationKey)
        {
            locationSprite = new PinLocationSprite(locationKey);
        }
        else
        {
            locationSprite = SD.Of(placement).Get(InteropProperties.LocationPinSprite);
        }

        locationSpriteScale =
            GetPinSpriteScale(locationSprite, SD.Of(placement).Get(InteropProperties.LocationPinSpriteSize));

        itemPoolGroups = [];
        itemSprites = [];
        foreach (var item in placement.Items)
        {
            itemPoolGroups[item] = SD.Of(item).Get(InjectedProps.ItemPoolGroup);
            ISprite sprite;
            if (SD.Of(item).Get(InteropProperties.ItemPinSpriteKey) is string itemKey)
            {
                sprite = new PinItemSprite(itemKey);
            }
            else
            {
                sprite = SD.Of(item).Get(InteropProperties.ItemPinSprite);
            }

            itemSprites[item] = (sprite,
                GetPinSpriteScale(sprite, SD.Of(item).Get(InteropProperties.ItemPinSpriteSize)));
        }

        HighlightScenes = SD.Of(placement).Get(InteropProperties.HighlightScenes);

        if (HighlightScenes is not null)
        {
            HighlightRooms = [];

            foreach (var scene in HighlightScenes)
            {
                if (!TransitionRoomSelector.Instance.Objects.TryGetValue(scene, out var rooms))
                {
                    continue;
                }

                foreach (var room in rooms)
                {
                    HighlightRooms.Add(room);
                }
            }
        }

        hints = SD.Of(placement).Get(InteropProperties.LocationHints).Select(ArchipelagoMapMod.LS.TrackerData.lm.CreateDNFLogicDef).ToArray();

        // This has default behaviour when the CoordinateLocation exists and no other properties are provided
        if (!MapChanger.Finder.TryGetLocation(placement.Name, out var _)
            && !SD.Of(placement).IsNonDefault(InteropProperties.MapLocations)
            && !SD.Of(placement).IsNonDefault(InteropProperties.AbsMapLocation)
            && !SD.Of(placement).IsNonDefault(InteropProperties.PinGridIndex)
            && SD.Of(placement).Get(InteropProperties.WorldMapLocations) is (string, float, float)[] worldMapLocations)
        {
            WorldMapPosition wmp = new(worldMapLocations);
            MapPosition = wmp;
            MapZone = wmp.MapZone;
        }
        // This doesn't have a default handler and will always fall through when the property is not provided
        else if (SD.Of(placement).Get(InteropProperties.AbsMapLocation) is (float, float) absMapLocation)
        {
            MapPosition = new AbsMapPosition(absMapLocation);
        }
        // This has a default handler and might not fall through when the property is not provided
        else if (SD.Of(placement).Get(InteropProperties.MapLocations) is (string, float, float)[] mapLocations)
        {
            MapRoomPosition mlp = new(mapLocations);
            MapPosition = mlp;
            MapZone = mlp.MapZone;
        }
        // This has a default handler
        else
        {
            PinGridIndex = SD.Of(placement).Get(InteropProperties.PinGridIndex);
            APmmPinManager.GridPins.Add(this);
        }
    }

    private float GetPinSpriteScale(ISprite sprite, (int, int)? interopSize)
    {
        if (interopSize is (int width, int height))
        {
            return SpriteManager.DEFAULT_PIN_SPRITE_SIZE / ((width + height) / 2f);
        }

        if (sprite is PinLocationSprite or PinItemSprite || sprite.Value is null)
        {
            return 1f;
        }

        return SpriteManager.DEFAULT_PIN_SPRITE_SIZE / ((sprite.Value.rect.width + sprite.Value.rect.height) / 2f);
    }

    private protected override bool ActiveBySettings()
    {
        if (Interop.HasBenchwarp() && ArchipelagoMapMod.GS.ShowBenchwarpPins && IsVisitedBench())
        {
            return true;
        }

        if (ArchipelagoMapMod.LS.GroupBy == GroupBySetting.Item)
        {
            foreach (var poolGroup in remainingItems.Select(item => itemPoolGroups[item]))
            {
                var poolState = ArchipelagoMapMod.LS.GetPoolGroupSetting(poolGroup);

                if (poolState == PoolState.On ||
                    (poolState == PoolState.Mixed && ArchipelagoMapMod.LS.RandomizedOn))
                {
                    return true;
                }
            }

            return false;
        }

        {
            var poolState = ArchipelagoMapMod.LS.GetPoolGroupSetting(LocationPoolGroup);

            return poolState == PoolState.On || (poolState == PoolState.Mixed && ArchipelagoMapMod.LS.RandomizedOn);
        }
    }

    private protected override bool ActiveByProgress()
    {
        if (Interop.HasBenchwarp() && ArchipelagoMapMod.GS.ShowBenchwarpPins && IsVisitedBench())
        {
            return true;
        }

        return (placementState is not RPS.Cleared &&
                (placementState is not RPS.ClearedPersistent || ArchipelagoMapMod.GS.ShowPersistentPins))
               || ArchipelagoMapMod.GS.ShowClearedPins;
    }

    public override void OnMainUpdate(bool active)
    {
        itemIndex = 0;

        UpdateRemainingItems();

        showItemSprite = remainingItems.Any()
                         && (ArchipelagoMapMod.LS.SpoilerOn
                             || (placementState is RPS.PreviewedUnreachable or RPS.PreviewedReachable &&
                                 placement.CanPreview())
                             || placementState is RPS.ClearedPersistent);

        StopPeriodicUpdate();

        if (showItemSprite && active)
        {
            StartPeriodicUpdate();
        }

        base.OnMainUpdate(active);
    }

    private protected override void UpdatePinSprites()
    {
        string pinShape = placementState switch
        {
            RPS.PreviewedReachable or RPS.PreviewedUnreachable => "Diamond",
            RPS.ClearedPersistent => "Hexagon",
            _ => "Circle"
        };
        BackgroundSprite = PinSpriteManager.GetSprite($"Background{pinShape}", false);
        BorderSprite = PinSpriteManager.GetSprite($"Border{pinShape}", false);

        if (showItemSprite
            && itemSprites.TryGetValue(remainingItems.ElementAt(itemIndex),
                out (ISprite itemSprite, float scale) spriteInfo))
        {
            Sprite = spriteInfo.itemSprite.Value;
            Sr.transform.localScale = new Vector3(spriteInfo.scale, spriteInfo.scale, 1f);

        }
        else
        {
            Sprite = locationSprite.Value;
            Sr.transform.localScale = new Vector3(locationSpriteScale, locationSpriteScale, 1f);
        }
    }

    private protected override void UpdatePinSize()
    {
        var size = pinSizes[ArchipelagoMapMod.GS.PinSize];

        if (Selected)
        {
            size *= SELECTED_MULTIPLIER;
        }
        else if (ArchipelagoMapMod.GS.ReachablePins
                 && placementState is RPS.UncheckedUnreachable or RPS.ClearedPersistent or RPS.Cleared)
        {
            size *= UNREACHABLE_SIZE_MULTIPLIER;
        }

        Size = size;
    }

    private protected override void UpdatePinColor()
    {
        Vector4 color = UnityEngine.Color.white;

        if (ArchipelagoMapMod.GS.ReachablePins
            && placementState is RPS.UncheckedUnreachable or RPS.PreviewedUnreachable)
        {
            Color = new Vector4(color.x * UNREACHABLE_COLOR_MULTIPLIER, color.y * UNREACHABLE_COLOR_MULTIPLIER,
                color.z * UNREACHABLE_COLOR_MULTIPLIER, color.w);
            return;
        }

        Color = color;
    }

    private protected override void UpdateBorderColor()
    {
        var color = placementState switch
        {
            RPS.OutOfLogicReachable => APmmColors.GetColor(APmmColorSetting.Pin_Out_of_logic),
            RPS.PreviewedUnreachable or RPS.PreviewedReachable => APmmColors.GetColor(APmmColorSetting.Pin_Previewed),
            RPS.Cleared => APmmColors.GetColor(APmmColorSetting.Pin_Cleared),
            RPS.ClearedPersistent => APmmColors.GetColor(APmmColorSetting.Pin_Persistent),
            _ => APmmColors.GetColor(APmmColorSetting.Pin_Normal)
        };

        if (placementState is RPS.UncheckedUnreachable or RPS.PreviewedUnreachable)
        {
            BorderColor = new Vector4(color.x * UNREACHABLE_COLOR_MULTIPLIER, color.y * UNREACHABLE_COLOR_MULTIPLIER,
                color.z * UNREACHABLE_COLOR_MULTIPLIER, color.w);
        }
        else
        {
            BorderColor = color;
        }
    }

    internal void UpdatePlacementState()
    {
        if (ArchipelagoMapMod.LS.TrackerData.clearedLocations.Contains(name))
        {
            if (placement.IsPersistent() && LocationPoolGroup is not "Benches")
            {
                placementState = RPS.ClearedPersistent;
            }
            else
            {
                placementState = RPS.Cleared;
            }
        }
        // Does not guarantee the item sprites should show (for a cost-only or a "none" preview)
        else if (ArchipelagoMapMod.LS.TrackerData.previewedLocations.Contains(name)
            || placement.Items.Any(item => (item.GetTag<ArchipelagoItemTag>()?.Hinted).GetValueOrDefault(false)))
        {
            if (Logic is not null && Logic.CanGet(ArchipelagoMapMod.LS.TrackerData.pm))
            {
                placementState = RPS.PreviewedReachable;
            }
            else
            {
                placementState = RPS.PreviewedUnreachable;
            }
        }
        else if (ArchipelagoMapMod.LS.TrackerData.uncheckedReachableLocations.Contains(name))
        {
            placementState = RPS.UncheckedReachable;
        }
        else
        {
            placementState = RPS.UncheckedUnreachable;
        }

        UpdateRemainingItems();
    }

    private void UpdateRemainingItems()
    {
        if (placementState is RPS.Cleared)
        {
            remainingItems = placement.Items;
        }
        else if (ArchipelagoMapMod.GS.ShowPersistentPins)
        {
            remainingItems = placement.Items.Where(item => !item.WasEverObtained() || item.IsPersistent());
        }
        else
        {
            remainingItems = placement.Items.Where(item => !item.WasEverObtained());
        }
    }

    internal override string GetSelectionText()
    {
        var text = base.GetSelectionText();

        if (HighlightRooms is not null)
        {
            text += $"\n\nRooms:";

            foreach (var scene in HighlightScenes)
            {
                text += $" {scene},";
            }

            text = text.Substring(0, text.Length - 1);
        }

        text += $"\n\nStatus:";

        text += placementState switch
        {
            RPS.UncheckedUnreachable => " Randomized, unchecked, unreachable",
            RPS.UncheckedReachable => " Randomized, unchecked, reachable",
            RPS.OutOfLogicReachable => " Randomized, unchecked, reachable through sequence break",
            RPS.PreviewedUnreachable => " Randomized, previewed, unreachable",
            RPS.PreviewedReachable => $" Randomized, previewed, reachable",
            RPS.Cleared => $" Cleared",
            RPS.ClearedPersistent => $" Randomized, cleared, persistent",
            _ => ""
        };

        if (Interop.HasBenchwarp() && LocationPoolGroup is "Benches")
        {
            if (BenchwarpInterop.IsVisitedBench(name))
            {
                text += ", can warp";
            }
            else
            {
                text += ", cannot warp";
            }
        }

        text += $"\n\nLogic: {Logic?.InfixSource ?? "not found"}";

        if ((placementState is RPS.PreviewedUnreachable or RPS.PreviewedReachable) && placement.TryGetPreviewText(out List<string> previewText))
        {
            text += $"\n\nPreviewed item(s):\n";

            foreach (string preview in previewText)
            {
                text += $"  {ToCleanPreviewText(preview)}\n";
            }

            text = text.Substring(0, text.Length - 1);
        }

        var obtainedItems = placement.Items.Where(item => item.WasEverObtained());

        if (obtainedItems.Any())
        {
            text += $"\n\nObtained item(s):";

            foreach (var item in obtainedItems)
            {
                text += $" {item.GetPreviewName()},";
            }

            text = text.Substring(0, text.Length - 1);
        }

        var spoilerItems = placement.Items.Where(item => !item.WasEverObtained());

        if (spoilerItems.Any() && ArchipelagoMapMod.LS.SpoilerOn
                               && !(placementState is RPS.PreviewedUnreachable or RPS.PreviewedReachable &&
                                    placement.CanPreview()))
        {
            text += $"\n\nSpoiler item(s):";

            foreach (var item in spoilerItems)
            {
                text += $" {item.GetPreviewName()},";
            }

            text = text.Substring(0, text.Length - 1);
        }

        return text;

        static string ToCleanPreviewText(string text)
        {
            return text.Replace("Pay ", "")
                .Replace("Once you own ", "")
                .Replace(", I'll gladly sell it to you.", "")
                .Replace("Requires ", "");
        }
    }

    internal override bool IsVisitedBench()
    {
        return BenchwarpInterop.IsVisitedBench(name);
    }
}