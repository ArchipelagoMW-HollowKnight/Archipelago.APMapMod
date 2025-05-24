using RandomizerCore;

namespace ArchipelagoMapMod.RC
{
    public readonly record struct ItemPlacement(APItem Item, APLocation Location)
    {
        /// <summary>
        /// The index of the item placement in the RandoModContext item placements. Initialized to -1 if the placement is not part of the ctx.
        /// </summary>
        public int Index { get; init; } = -1;

        public void Deconstruct(out APItem item, out APLocation location)
        {
            item = Item;
            location = Location;
        }

        public static implicit operator GeneralizedPlacement(ItemPlacement p) => new(p.Item, p.Location);
        public static explicit operator ItemPlacement(GeneralizedPlacement p) => new((APItem)p.Item, (APLocation)p.Location);
    }
}
