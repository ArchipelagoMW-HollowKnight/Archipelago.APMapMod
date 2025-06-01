using AP = Archipelago.HollowKnight.ArchipelagoMod;

namespace ArchipelagoMapMod.Settings;

public class NoveltySettings
{
    public bool RandomizeSwim = AP.Instance.SlotData.Options.RandomizeSwim;
    public bool RandomizeElevatorPass = AP.Instance.SlotData.Options.RandomizeElevatorPass;
    public bool RandomizeNail = AP.Instance.SlotData.Options.RandomizeNail;
    public bool RandomizeFocus = AP.Instance.SlotData.Options.RandomizeFocus;
    public bool SplitClaw = AP.Instance.SlotData.Options.SplitMantisClaw;
    public bool SplitCloak = AP.Instance.SlotData.Options.SplitMothwingCloak;
    public bool SplitSuperdash = AP.Instance.SlotData.Options.SplitCrystalHeart;
    public bool EggShop = AP.Instance.SlotData.Options.EggShopSlots > 0;
}
