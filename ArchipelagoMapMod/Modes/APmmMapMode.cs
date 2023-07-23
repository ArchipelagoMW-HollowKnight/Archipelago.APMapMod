using MapChanger;

namespace ArchipelagoMapMod.Modes;

internal class apmmMapMode : MapMode
{
    public override float Priority => 0f;
    public override bool ForceHasMap => true;
    public override bool ForceHasQuill => ArchipelagoMapMod.GS.AlwaysHaveQuill;
    public override bool? VanillaPins => false;
    public override bool? MapMarkers => ArchipelagoMapMod.GS.ShowMapMarkers ? null : false;
    public override bool ImmediateMapUpdate => true;
    public override bool FullMap => true;
}