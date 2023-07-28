using Archipelago.HollowKnight.IC;
using Archipelago.MultiClient.Net.Enums;
using ArchipelagoMapMod.IC;
using ConnectionMetadataInjector;
using ItemChanger;
using UnityEngine;

namespace ArchipelagoMapMod.Pins;

/// <summary>
///     Fetches only the "normal" sprite. Appropriate for if spoilers are on or items are being previewed.
/// </summary>
internal class PinItemSprite : ISprite
{
    /// <summary>
    ///     Sets the sprite based on an item's PoolGroup.
    /// </summary>
    public PinItemSprite(AbstractItem item)
    {
        Key = SupplementalMetadata.Of(item).Get(InjectedProps.ItemPoolGroup);

        if (Key == "Other")
        {
            if (item.GetTag(out ArchipelagoItemTag apTag))
            {
                if (apTag.Flags == ItemFlags.Advancement)
                    Value = PinSprite.APProgression.Value;
                else if (apTag.Flags == ItemFlags.NeverExclude)
                    Value = PinSprite.APUseful.Value;
                else if (apTag.Flags == ItemFlags.None)
                    Value = PinSprite.APTrash.Value;
            }
            else
            {
                Value = PinSpriteManager.GetSprite(Key, true);
            }
        }
        else
        {
            Value = PinSpriteManager.GetSprite(Key, true);
        }
    }

    /// <summary>
    ///     Sets the sprite based on a connection-provided key.
    /// </summary>
    public PinItemSprite(string key)
    {
        Key = key;

        Value = PinSpriteManager.GetSprite(Key, false);
    }

    public string Key { get; }

    public Sprite Value { get; }

    public ISprite Clone()
    {
        return (ISprite) MemberwiseClone();
    }
}