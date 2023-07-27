using ArchipelagoMapMod.RC;
using RandomizerCore.Logic;

namespace ArchipelagoMapMod.RandomizerData
{
    public record CostDef(string Term, int Amount)
    {
        public virtual LogicCost ToLogicCost(LogicManager lm)
        {
            return Term switch
            {
                "GEO" => new LogicGeoCost(lm, Amount),
                _ => new SimpleCost(lm.GetTermStrict(Term), Amount),
            };
        }
    }
}
