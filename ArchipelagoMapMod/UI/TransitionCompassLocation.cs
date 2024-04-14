using MapChanger.Defs;
using UnityEngine;

namespace ArchipelagoMapMod.UI
{
    internal class TransitionCompassLocation(GameObject go, Sprite sprite, Vector4 color) : GameObjectCompassLocation(go)
    {
        public override Sprite Sprite => sprite;

        public override Color Color => color;
    }
}
