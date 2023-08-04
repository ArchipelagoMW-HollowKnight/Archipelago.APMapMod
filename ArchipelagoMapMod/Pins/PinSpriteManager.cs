﻿using ArchipelagoMapMod.Settings;
using MapChanger;
using UnityEngine;

namespace ArchipelagoMapMod.Pins;

internal static class PinSpriteManager
{
    internal static readonly Dictionary<string, string> MapChangerSpriteKeys = new()
    {
        {"Dreamers", "Dreamer"},
        {"Skills", "Skill"},
        {"Charms", "Charm"},
        {"Keys", "Key"},
        {"Mask Shards", "Mask"},
        {"Vessel Fragments", "Vessel"},
        {"Charm Notches", "Notch"},
        {"Pale Ore", "Ore"},
        {"Geo Chests", "Geo"},
        {"Rancid Eggs", "Egg"},
        {"Relics", "Relic"},
        {"Whispering Roots", "Root"},
        {"Boss Essence", "EssenceBoss"},
        {"Grubs", "Grub"},
        {"Mimics", "Grub"},
        {"Maps", "Map"},
        {"Stags", "Stag"},
        {"Lifeblood Cocoons", "Cocoon"},
        {"Grimmkin Flames", "Flame"},
        {"Journal Entries", "Journal"},
        {"Geo Rocks", "Rock"},
        {"Boss Geo", "Geo"},
        {"Soul Totems", "Totem"},
        {"Lore Tablets", "Lore"},
        {"Shops", "Shop"},
        {"Levers", "Lever"},
        {"Mr Mushroom", "Lore"},
        {"Benches", "Bench"},
        {"Other", "Unknown"}
    };

    internal static Sprite GetSprite(string key, bool defaultToUnknown)
    {
        if (MapChangerSpriteKeys.TryGetValue(key, out var builtInKey))
            return SpriteManager.Instance.GetSprite($"Pins.{builtInKey}");

        if (defaultToUnknown) return SpriteManager.Instance.GetSprite("Pins.Unknown");

        return SpriteManager.Instance.GetSprite($"Pins.{key}");
    }

    internal static Sprite GetStyleDependentSprite(string key, PinStyle style, bool defaultToUnknown)
    {
        var builtInKey = "Unknown";

        if (style is PinStyle.Normal)
            return GetSprite(key, defaultToUnknown);
        // Other PinStyles always default to Unknown
        if (style is PinStyle.Q_Marks_1)
            builtInKey = key switch
            {
                "Shops" => "Shop",
                "Benches" => "Bench",
                _ => "Unknown"
            };
        else if (style is PinStyle.Q_Marks_2)
            builtInKey = key switch
            {
                "Grubs" => "UnknownGrubInv",
                "Mimics" => "UnknownGrubInv",
                "Lifeblood Cocoons" => "UnknownLifebloodInv",
                "Geo Rocks" => "UnknownGeoRockInv",
                "Soul Totems" => "UnknownTotemInv",
                "Shops" => "Shop",
                "Benches" => "Bench",
                _ => "Unknown"
            };
        else if (style is PinStyle.Q_Marks_3)
            builtInKey = key switch
            {
                "Grubs" => "UnknownGrub",
                "Mimics" => "UnknownGrub",
                "Lifeblood Cocoons" => "UnknownLifeblood",
                "Geo Rocks" => "UnknownGeoRock",
                "Soul Totems" => "UnknownTotem",
                "Shops" => "Shop",
                "Benches" => "Bench",
                _ => "Unknown"
            };

        return SpriteManager.Instance.GetSprite($"Pins.{builtInKey}");
    }
}