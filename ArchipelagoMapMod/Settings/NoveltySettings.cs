using AP = Archipelago.HollowKnight.Archipelago;

namespace ArchipelagoMapMod.Settings
{
    public class NoveltySettings
    {
        public bool RandomizeSwim = AP.Instance.SlotOptions.RandomizeSwim;
        public bool RandomizeElevatorPass = AP.Instance.SlotOptions.RandomizeElevatorPass;
        public bool RandomizeNail = AP.Instance.SlotOptions.RandomizeNail;
        public bool RandomizeFocus = AP.Instance.SlotOptions.RandomizeFocus;
        public bool SplitClaw = AP.Instance.SlotOptions.SplitMantisClaw;
        public bool SplitCloak = AP.Instance.SlotOptions.SplitMothwingCloak;
        public bool SplitSuperdash = AP.Instance.SlotOptions.SplitCrystalHeart;
        public bool EggShop = AP.Instance.SlotOptions.EggShopSlots > 0;
    }
}
