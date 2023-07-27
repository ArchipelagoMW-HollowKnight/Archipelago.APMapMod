using RandomizerCore;

namespace ArchipelagoMapMod.RC
{
    public record struct TransitionPlacement(APTransition Target, APTransition Source)
    {
        public void Deconstruct(out APTransition target, out APTransition source)
        {
            target = this.Target;
            source = this.Source;
        }

        public static implicit operator GeneralizedPlacement(TransitionPlacement p) => new(p.Target, p.Source);
        public static explicit operator TransitionPlacement(GeneralizedPlacement p) => new((APTransition)p.Item, (APTransition)p.Location);
    }
}
