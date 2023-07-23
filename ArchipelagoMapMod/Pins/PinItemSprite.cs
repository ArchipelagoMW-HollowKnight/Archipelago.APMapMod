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

        Value = PinSpriteManager.GetSprite(Key, true);
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