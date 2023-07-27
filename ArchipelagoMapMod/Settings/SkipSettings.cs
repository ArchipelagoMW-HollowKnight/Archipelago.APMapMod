namespace ArchipelagoMapMod.Settings;
using AP = Archipelago.HollowKnight.Archipelago;

public class SkipSettings
{
    public bool PreciseMovement = AP.Instance.SlotOptions.PreciseMovement;
    public bool ProficientCombat = AP.Instance.SlotOptions.ProficientCombat;
    public bool BackgroundObjectPogos = AP.Instance.SlotOptions.BackgroundObjectPogos;
    public bool EnemyPogos = AP.Instance.SlotOptions.EnemyPogos;
    public bool ObscureSkips = AP.Instance.SlotOptions.ObscureSkips;
    public bool ShadeSkips = AP.Instance.SlotOptions.ShadeSkips;
    public bool InfectionSkips = AP.Instance.SlotOptions.InfectionSkips;
    public bool FireballSkips = AP.Instance.SlotOptions.FireballSkips;
    public bool SpikeTunnels = AP.Instance.SlotOptions.SpikeTunnels;
    public bool AcidSkips = AP.Instance.SlotOptions.AcidSkips;
    public bool DamageBoosts = AP.Instance.SlotOptions.DamageBoosts;
    public bool DangerousSkips = AP.Instance.SlotOptions.DangerousSkips;
    public bool DarkRooms = AP.Instance.SlotOptions.DarkRooms;
    public bool ComplexSkips = AP.Instance.SlotOptions.ComplexSkips;
    public bool DifficultSkips = AP.Instance.SlotOptions.DifficultSkips;
    
    //TODO: enable this when AP adds relevant logic settings.
    public bool Slopeballs = false;
    public bool ShriekPogos = false;
}