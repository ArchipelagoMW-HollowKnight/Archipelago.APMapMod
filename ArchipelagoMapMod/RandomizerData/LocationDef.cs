﻿using Newtonsoft.Json;

namespace ArchipelagoMapMod.RandomizerData
{
    public record LocationDef
    {
        public string Name { get; init; }
        public string SceneName { get; init; }
        [JsonIgnore] public virtual string TitledArea { get => Data.GetRoomDef(SceneName)?.TitledArea; }
        [JsonIgnore] public virtual string MapArea { get => Data.GetRoomDef(SceneName)?.MapArea; }
        /// <summary>
        /// If true, copies of this location after the first may be shuffled among other flexible count locations by the RequestBuilder.
        /// </summary>
        public bool FlexibleCount { get; init; }
        /// <summary>
        /// If true, copies of this location after the first will receive severe penalties to prevent multiple progression items.
        /// </summary>
        public bool AdditionalProgressionPenalty { get; init; }
    }

    // Incomplete ideas for potential enhancements below:
    /*
    [Flags]
    public enum LocationPoolFlags : long
    {
        None = 0L,
        IsSkill = 1L << 0,
        IsKey = 1L << 1,
        IsCharm = 1L << 2,
        IsMask = 1L << 3,
        IsVessel = 1L << 4,
        IsNotch = 1L << 5,
        IsOre = 1L << 6,
        IsGeoChest = 1L << 7,
        IsEgg = 1L << 8,
        IsRelic = 1L << 9,
        IsRoot = 1L << 10,
        IsDreamWarriorEssence = 1L << 11,
        IsDreamBossEssence = 1L << 12,
        IsGrub = 1L << 13,
        IsMimic = 1L << 14,
        IsMap = 1L << 15,
        IsStag = 1L << 16,
        IsCocoon = 1L << 17,
        IsFlame = 1L << 18,
        IsHunterJournalEntry = 1L << 19,
        IsRock = 1L << 20,
        IsSoul = 1L << 21,
        IsLore = 1L << 22,
        IsCustomAbility = 1L << 23,
        IsDreamer = 1L << 24, // oops
    }

    [Flags]
    public enum LocationPropertyFlags : long
    {
        None,

        IsChest,
        IsShop,
    }
    */
}
