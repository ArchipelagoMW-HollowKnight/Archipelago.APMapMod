using RandomizerCore.Logic;

namespace ArchipelagoMapMod.RC;

public class LogicGeoCost : LogicCost
{
    public Term CanReplenishGeoWaypoint { get; init; }
    public int GeoAmount { get; set; }

    public LogicGeoCost() { }

    public LogicGeoCost(LogicManager lm, int amount)
    {
        CanReplenishGeoWaypoint = lm.GetTermStrict("Can_Replenish_Geo");
        GeoAmount = amount;
    }

    public override bool CanGet(ProgressionManager pm)
    {
        return pm.Has(CanReplenishGeoWaypoint.Id);
    }

    public override IEnumerable<Term> GetTerms()
    {
        yield return CanReplenishGeoWaypoint;
    }

    public override string ToString()
    {
        return $"LogicGeoCost {{{GeoAmount} Geo}}";
    }

}
