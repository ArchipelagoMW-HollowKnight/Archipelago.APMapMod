using ItemChanger;
using ItemChanger.Internal;
using UnityEngine;

namespace ArchipelagoMapMod.IC;

public class PinSprite : ISprite
{
    private static readonly SpriteManager pinManager = new(typeof(EmbeddedSprite).Assembly, "ArchipelagoMapMod.Resources.Sprites.Pins.");

    public static PinSprite APProgression = new("pinAPProgression");
    public static PinSprite APUseful = new("pinAPUseful");
    public static PinSprite APTrash = new("pinAP");
    
    private string PinName { get; }

    
    public PinSprite(string pinName)
    {
        PinName = pinName;
    }
    
    public ISprite Clone() => (ISprite) MemberwiseClone();
    
    [Newtonsoft.Json.JsonIgnore]
    public Sprite Value => pinManager.GetSprite(PinName);
}