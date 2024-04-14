
using RandomizerCore;
using RandomizerCore.Logic;
using ArchipelagoMapMod.RandomizerData;
using ArchipelagoMapMod.Settings;

namespace ArchipelagoMapMod.RC
{
    #nullable enable
    public class ProgressionInitializer : ILogicItem
    {
        /// <summary>
        /// Event invoked after base randomizer term modifiers are added to the initializer.
        /// </summary>
        public static event Action<LogicManager, GenerationSettings, ProgressionInitializer>? OnCreateProgressionInitializer;

        public ProgressionInitializer() { }
        public ProgressionInitializer(LogicManager lm, GenerationSettings gs, StartDef startDef)
        {
            foreach (string setting in Data.GetApplicableLogicSettings(gs))
            {
                Setters.Add(new TermValue(lm.GetTermStrict(setting), 1));
            }

            Setters.Add(new TermValue(lm.GetTermStrict(gs.TransitionSettings.Mode switch
            {
                TransitionSettings.TransitionMode.None => "ITEMRANDO",
                TransitionSettings.TransitionMode.MapAreaRandomizer => "MAPAREARANDO",
                TransitionSettings.TransitionMode.FullAreaRandomizer => "FULLAREARANDO",
                _ => "ROOMRANDO",
            }), 1));

            foreach (TermValue tv in startDef.GetStartLocationProgression(lm))
            {
                if (tv.Term.Type == TermType.State) StartStateLinkedTerms.Add(tv.Term);
                else Setters.Add(tv);
            }

            Setters.Add(new TermValue(lm.GetTermStrict("GRUBS"), -gs.CostSettings.GrubTolerance));
            Setters.Add(new TermValue(lm.GetTermStrict("ESSENCE"), -gs.CostSettings.EssenceTolerance));
            Setters.Add(new TermValue(lm.GetTermStrict("RANCIDEGGS"), -gs.CostSettings.EggTolerance));
            Setters.Add(new TermValue(lm.GetTermStrict("CHARMS"), -gs.CostSettings.CharmTolerance));

            Setters.Add(new TermValue(lm.GetTermStrict("MASKSHARDS"), 20 - 4 * gs.CursedSettings.CursedMasks));
            Setters.Add(new TermValue(lm.GetTermStrict("NOTCHES"), 3 - gs.CursedSettings.CursedNotches));

            StartStateTerm = lm.GetTerm("Start_State");

            try
            {
                OnCreateProgressionInitializer?.Invoke(lm, gs, this);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Error invoking OnCreateProgressionInitializer", e);
            }
        }

        public List<TermValue> Setters = [];
        public List<TermValue> Increments = [];
        public List<Term> StartStateLinkedTerms = [];
        public Term? StartStateTerm;

        public string Name => "Progression Initializer";

        public void AddTo(ProgressionManager pm)
        {
            foreach (TermValue tv in Setters) pm.Set(tv);
            foreach (TermValue tv in Increments) pm.Incr(tv);
            if (StartStateTerm is not null && !pm.mu.HasCustomLongTermRevertPoint)
            {
                foreach (Term t in StartStateLinkedTerms) pm.mu.LinkState(StartStateTerm, t);
            }
        }

        public IEnumerable<Term> GetAffectedTerms()
        {
            foreach (TermValue tv in Setters) yield return tv.Term;
            foreach (TermValue tv in Increments) yield return tv.Term;
            if (StartStateTerm is not null) yield return StartStateTerm;
        }
    }
}
