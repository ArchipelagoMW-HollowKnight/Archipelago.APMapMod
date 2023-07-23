using ItemChanger;
using MapChanger;
using UnityEngine;

namespace ArchipelagoMapMod;

/// <summary>
///     Uses MapChanger's SpriteManager to get a Sprite.
/// </summary>
internal class EmbeddedSprite : ISprite
{
    internal EmbeddedSprite(string key)
    {
        Key = key;
    }

    internal string Key { get; init; }
    public Sprite Value => SpriteManager.Instance.GetSprite(Key);

    public ISprite Clone()
    {
        return (ISprite) MemberwiseClone();
    }
}