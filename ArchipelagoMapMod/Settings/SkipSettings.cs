namespace ArchipelagoMapMod.Settings;
using AP = Archipelago.HollowKnight.ArchipelagoMod;

public class SkipSettings
{
    public bool PreciseMovement = AP.Instance.SlotData.Options.PreciseMovement;
    public bool ProficientCombat = AP.Instance.SlotData.Options.ProficientCombat;
    public bool BackgroundObjectPogos = AP.Instance.SlotData.Options.BackgroundObjectPogos;
    public bool EnemyPogos = AP.Instance.SlotData.Options.EnemyPogos;
    public bool ObscureSkips = AP.Instance.SlotData.Options.ObscureSkips;
    public bool ShadeSkips = AP.Instance.SlotData.Options.ShadeSkips;
    public bool InfectionSkips = AP.Instance.SlotData.Options.InfectionSkips;
    public bool FireballSkips = AP.Instance.SlotData.Options.FireballSkips;
    public bool SpikeTunnels = AP.Instance.SlotData.Options.SpikeTunnels;
    public bool AcidSkips = AP.Instance.SlotData.Options.AcidSkips;
    public bool DamageBoosts = AP.Instance.SlotData.Options.DamageBoosts;
    public bool DangerousSkips = AP.Instance.SlotData.Options.DangerousSkips;
    public bool DarkRooms = AP.Instance.SlotData.Options.DarkRooms;
    public bool ComplexSkips = AP.Instance.SlotData.Options.ComplexSkips;
    public bool DifficultSkips = AP.Instance.SlotData.Options.DifficultSkips;

    //TODO: enable this when AP adds relevant logic settings.
    public bool Slopeballs = false;
    public bool ShriekPogos = false;
}