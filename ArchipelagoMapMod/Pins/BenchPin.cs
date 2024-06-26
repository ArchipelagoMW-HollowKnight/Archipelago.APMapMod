﻿using ArchipelagoMapMod.RC;
using ItemChanger;
using ItemChanger.Internal;
using MapChanger;
using MapChanger.Defs;
using UnityEngine;
using Finder = MapChanger.Finder;

namespace ArchipelagoMapMod.Pins;

internal sealed class BenchPin : APmmPin
{
    private static readonly ISprite benchSprite = new PinLocationSprite("Benches");
    internal override HashSet<string> ItemPoolGroups => ["Benches"];

    internal string BenchName { get; private set; }

    internal void Initialize(string benchName, string sceneName)
    {
        Initialize();

        BenchName = benchName;
        SceneName = sceneName;

        LocationPoolGroup = "Benches";
        
        if (benchName is BenchwarpInterop.BENCH_WARP_START)
        {
            var start = APLogicSetup.Context.StartDef;

            if (Finder.IsMappedScene(SceneName))
            {
                WorldMapPosition wmp = new(new (string, float, float)[] {(SceneName, start.X, start.Y)});
                MapPosition = wmp;
                MapZone = wmp.MapZone;
            }
            else
            {
                var mappedScene = Finder.GetMappedScene(SceneName);
                MapRoomPosition mapPosition = new(new (string, float, float)[] {(mappedScene, 0, 0)});
                MapPosition = mapPosition;
                MapZone = mapPosition.MapZone;
            }
        }
        else if (InteropProperties.GetDefaultMapLocations(benchName) is (string, float, float)[] mapLocations)
        {
            MapRoomPosition mapPosition = new(mapLocations);
            MapPosition = mapPosition;
            MapZone = mapPosition.MapZone;
        }
        else
        {
            APmmPinManager.GridPins.Add(this);
        }
    }

    private protected override bool ActiveBySettings()
    {
        return ArchipelagoMapMod.GS.ShowBenchwarpPins;
    }

    private protected override bool ActiveByProgress()
    {
        return true;
    }

    private protected override void UpdatePinSprites()
    {
        Sprite = benchSprite.Value;
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
        if (IsVisitedBench())
            Color = UnityEngine.Color.white;
        else
            Color = new Vector4(UNREACHABLE_COLOR_MULTIPLIER, UNREACHABLE_COLOR_MULTIPLIER,
                UNREACHABLE_COLOR_MULTIPLIER, 1f);
    }

    private protected override void UpdateBorderColor()
    {
        Vector4 color;

        color = APmmColors.GetColor(APmmColorSetting.Pin_Normal);

        if (!IsVisitedBench())
        {
            color.x *= UNREACHABLE_COLOR_MULTIPLIER;
            color.y *= UNREACHABLE_COLOR_MULTIPLIER;
            color.z *= UNREACHABLE_COLOR_MULTIPLIER;
        }

        BorderColor = color;
    }

    internal override string GetSelectionText()
    {
        var text = $"{BenchName.ToCleanName()}";

        if (SceneName is not null) text += $"\n\nRoom: {SceneName}";

        text += "\n\nStatus:";

        if (IsVisitedBench())
            text += " Can warp";
        else
            text += " Cannot warp";

        return text;
    }

    internal override bool IsVisitedBench()
    {
        return BenchwarpInterop.IsVisitedBench(BenchName);
    }
}