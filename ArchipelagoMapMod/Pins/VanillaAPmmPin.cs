using ArchipelagoMapMod.Settings;
using ConnectionMetadataInjector.Util;
using ItemChanger;
using MapChanger;
using MapChanger.Defs;
using RandomizerCore;
using RandomizerMod.RandomizerData;
using UnityEngine;
using Finder = ItemChanger.Finder;
using L = RandomizerMod.Localization;
using RM = RandomizerMod.RandomizerMod;

namespace ArchipelagoMapMod.Pins;

internal sealed class VanillaapmmPin : apmmPin
{
    private static readonly Vector4 vanillaColor = new(UNREACHABLE_COLOR_MULTIPLIER, UNREACHABLE_COLOR_MULTIPLIER,
        UNREACHABLE_COLOR_MULTIPLIER, 1f);

    private ISprite locationSprite;

    internal override HashSet<string> ItemPoolGroups => new() {LocationPoolGroup};

    internal void Initialize(GeneralizedPlacement placement)
    {
        Initialize();

        SceneName = Data.GetLocationDef(name)?.SceneName ?? Finder.GetLocation(name)?.sceneName;

        LocationPoolGroup = SubcategoryFinder.GetLocationPoolGroup(placement.Location.Name).FriendlyName();
        locationSprite = new PinLocationSprite(LocationPoolGroup);

        if (InteropProperties.GetDefaultMapLocations(name) is (string, float, float)[] mapLocations)
        {
            MapRoomPosition mlp = new(mapLocations);
            MapPosition = mlp;
            MapZone = mlp.MapZone;
        }
        else
        {
            apmmPinManager.GridPins.Add(this);
        }

        hints = InteropProperties.GetDefaultLocationHints(placement.Location.Name)
            .Select(RM.RS.TrackerData.lm.CreateDNFLogicDef).ToArray();
    }

    private protected override bool ActiveBySettings()
    {
        var poolState = ArchipelagoMapMod.LS.GetPoolGroupSetting(LocationPoolGroup);

        return poolState == PoolState.On || (poolState == PoolState.Mixed && ArchipelagoMapMod.LS.VanillaOn);
    }

    private protected override bool ActiveByProgress()
    {
        return !Tracker.HasClearedLocation(name) || ArchipelagoMapMod.GS.ShowClearedPins;
    }

    private protected override void UpdatePinSprite()
    {
        Sprite = locationSprite.Value;
    }

    private protected override void UpdatePinSize()
    {
        Size = pinSizes[ArchipelagoMapMod.GS.PinSize];

        if (Selected)
            Size *= SELECTED_MULTIPLIER;
        else
            Size *= UNREACHABLE_SIZE_MULTIPLIER;
    }

    private protected override void UpdatePinColor()
    {
        Color = vanillaColor;
    }

    private protected override void UpdateBorderColor()
    {
        Vector4 color;

        if (Tracker.HasClearedLocation(name))
            color = apmmColors.GetColor(apmmColorSetting.Pin_Cleared);
        else if (IsPersistent())
            color = apmmColors.GetColor(apmmColorSetting.Pin_Persistent);
        else
            color = apmmColors.GetColor(apmmColorSetting.Pin_Normal);

        BorderColor = new Vector4(color.x * UNREACHABLE_COLOR_MULTIPLIER, color.y * UNREACHABLE_COLOR_MULTIPLIER,
            color.z * UNREACHABLE_COLOR_MULTIPLIER, color.w);
    }

    internal override string GetSelectionText()
    {
        var text = base.GetSelectionText();

        text += $"\n\n{L.Localize("Status")}:";

        if (Tracker.HasClearedLocation(name))
        {
            text += $" {L.Localize("Not randomized, cleared")}";
        }
        else
        {
            if (IsPersistent())
                text += $" {L.Localize("Not randomized, persistent")}";
            else
                text += $" {L.Localize("Not randomized, unchecked")}";
        }

        text += $"\n\n{L.Localize("Logic")}: {Logic?.InfixSource ?? "not found"}";

        return text;
    }

    private bool IsPersistent()
    {
        return LocationPoolGroup == PoolGroup.LifebloodCocoons.FriendlyName()
               || LocationPoolGroup == PoolGroup.SoulTotems.FriendlyName()
               || LocationPoolGroup == PoolGroup.LoreTablets.FriendlyName();
    }
}