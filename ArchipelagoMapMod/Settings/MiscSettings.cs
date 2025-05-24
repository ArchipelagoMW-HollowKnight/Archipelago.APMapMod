namespace ArchipelagoMapMod.Settings;

public class MiscSettings
{
    public bool RandomizeNotchCosts;
    public bool ExtraPlatforms;
    public SalubraNotchesSetting SalubraNotches;
    public MaskShardType MaskShards;
    public VesselFragmentType VesselFragments;
    public bool SteelSoul;
    public ToggleableFireballSetting FireballUpgrade;

    public enum MaskShardType
    {
        FourShardsPerMask,
        TwoShardsPerMask,
        OneShardPerMask
    }

    public enum VesselFragmentType
    {
        ThreeFragmentsPerVessel,
        TwoFragmentsPerVessel,
        OneFragmentPerVessel
    }

    public enum SalubraNotchesSetting
    {
        GroupedWithCharmNotchesPool,
        Vanilla,
        Randomized,
        AutoGivenAtCharmThreshold
    }

    public enum ToggleableFireballSetting
    {
        Normal,
        Deferred,
        Toggleable
    }
}

