using ArchipelagoMapMod.Pins;
using ArchipelagoMapMod.RC;
using Newtonsoft.Json;

namespace ArchipelagoMapMod.Settings;

public class LocalSettings
{
    public GroupBySetting GroupBy = GroupBySetting.Location;
    public bool InitializedPreviously;

    public Dictionary<string, PoolState> PoolSettings;
    public bool RandomizedOn = true;
    
    public TrackerData TrackerData;
    public TrackerData TrackerDataWithoutSequenceBreaks;

    public int ProfileID => GameManager.instance.profileID;

    public bool SpoilerOn;
    public bool VanillaOn;

    internal void Initialize()
    {
        if (InitializedPreviously) return;

        PoolSettings = APmmPinManager.AllPoolGroups.ToDictionary(poolGroup => poolGroup, poolGroup => PoolState.On);
        ResetPoolSettings();

        InitializedPreviously = true;
    }

    internal void ToggleGroupBy()
    {
        GroupBy = (GroupBySetting) (((int) GroupBy + 1) % Enum.GetNames(typeof(GroupBySetting)).Length);
        ResetPoolSettings();
    }

    internal void ToggleSpoilers()
    {
        SpoilerOn = !SpoilerOn;
    }

    internal void ToggleRandomized()
    {
        RandomizedOn = !RandomizedOn;
        ResetPoolSettings();
    }

    internal void ToggleVanilla()
    {
        VanillaOn = !VanillaOn;
        ResetPoolSettings();
    }

    internal PoolState GetPoolGroupSetting(string poolGroup)
    {
        if (PoolSettings.ContainsKey(poolGroup)) return PoolSettings[poolGroup];
        return PoolState.Off;
    }

    internal void SetPoolGroupSetting(string poolGroup, PoolState state)
    {
        if (PoolSettings.ContainsKey(poolGroup)) PoolSettings[poolGroup] = state;
    }

    internal void TogglePoolGroupSetting(string poolGroup)
    {
        if (!PoolSettings.ContainsKey(poolGroup)) return;

        PoolSettings[poolGroup] = PoolSettings[poolGroup] switch
        {
            PoolState.Off => PoolState.On,
            PoolState.On => PoolState.Off,
            PoolState.Mixed => PoolState.On,
            _ => PoolState.On
        };
    }

    /// <summary>
    ///     Reset the PoolGroups that are active based on the RandomizedOn, VanillaOn and Group By settings.
    ///     When an individual pool that by default contains a mixed of randomized/vanilla placements gets toggled,
    ///     It will either be forced to "On" or "Off" and the corresponding affected RandommizedOn/VanillaOn setting
    ///     appears as "Custom" in the UI.
    /// </summary>
    private void ResetPoolSettings()
    {
        foreach (var poolGroup in APmmPinManager.AllPoolGroups)
            SetPoolGroupSetting(poolGroup, GetResetPoolState(poolGroup));

        PoolState GetResetPoolState(string poolGroup)
        {
            bool IsRando;
            bool IsVanilla;

            if (GroupBy == GroupBySetting.Item)
            {
                IsRando = APmmPinManager.RandoItemPoolGroups.Contains(poolGroup);
                IsVanilla = APmmPinManager.VanillaItemPoolGroups.Contains(poolGroup);
            }
            else
            {
                IsRando = APmmPinManager.RandoLocationPoolGroups.Contains(poolGroup);
                IsVanilla = APmmPinManager.VanillaLocationPoolGroups.Contains(poolGroup);
            }

            if (IsRando && IsVanilla && ArchipelagoMapMod.LS.RandomizedOn != ArchipelagoMapMod.LS.VanillaOn)
                return PoolState.Mixed;
            if ((IsRando && RandomizedOn) || (IsVanilla && VanillaOn)) return PoolState.On;
            return PoolState.Off;
        }
    }
}