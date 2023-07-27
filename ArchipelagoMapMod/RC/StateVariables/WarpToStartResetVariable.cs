﻿using RandomizerCore.Logic;
using RandomizerCore.Logic.StateLogic;

namespace ArchipelagoMapMod.RC.StateVariables
{
    /*
     * Prefix: $WARPTOSTART
     * Required Parameters: none
     * Optional Parameters: none
     * Provides the effect of warping to start via Benchwarp or savequit.
    */
    public class WarpToStartResetVariable : StateModifier
    {
        public override string Name { get; }
        public const string Prefix = "$WARPTOSTART";

        protected readonly SaveQuitResetVariable SaveQuitReset;
        protected readonly StartRespawnResetVariable StartRespawnReset;

        public WarpToStartResetVariable(string name, LogicManager lm)
        {
            Name = name;
            try
            {
                SaveQuitReset = (SaveQuitResetVariable)lm.GetVariableStrict(SaveQuitResetVariable.Prefix);
                StartRespawnReset = (StartRespawnResetVariable)lm.GetVariableStrict(StartRespawnResetVariable.Prefix);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Error constructing WarpToStartResetVariable", e);
            }
        }

        public static bool TryMatch(LogicManager lm, string term, out LogicVariable variable)
        {
            if (VariableResolver.TryMatchPrefix(term, Prefix, out _))
            {
                variable = new WarpToStartResetVariable(term, lm);
                return true;
            }
            variable = default;
            return false;
        }

        public override IEnumerable<LazyStateBuilder>? ProvideState(object? sender, ProgressionManager pm)
        {
            return Enumerable.Empty<LazyStateBuilder>();
        }

        public override IEnumerable<LazyStateBuilder> ModifyState(object? sender, ProgressionManager pm, LazyStateBuilder state)
        {
            yield return ResetSingle(pm, state);
        }

        public override IEnumerable<Term> GetTerms()
        {
            return SaveQuitReset.GetTerms().Concat(StartRespawnReset.GetTerms());
        }

        public LazyStateBuilder ResetSingle(ProgressionManager pm, LazyStateBuilder state) => StartRespawnReset.ResetSingle(pm, SaveQuitReset.ResetSingle(pm, state));
    }
}
