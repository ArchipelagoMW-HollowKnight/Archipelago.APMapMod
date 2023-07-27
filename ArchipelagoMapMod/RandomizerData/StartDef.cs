using GlobalEnums;
using RandomizerCore;
using RandomizerCore.Logic;
using ArchipelagoMapMod.Settings;

namespace ArchipelagoMapMod.RandomizerData
{
    public record StartDef
    {
        /// <summary>
        /// The name of the start. Names should be unique.
        /// </summary>
        public string Name { get; init; }
        /// <summary>
        /// The scene of the start location in-game.
        /// </summary>
        public string SceneName { get; init; }
        /// <summary>
        /// The x-coordinate of the start location in-game.
        /// </summary>
        public float X { get; init; }
        /// <summary>
        /// The y-coordinate of the start location in-game.
        /// </summary>
        public float Y { get; init; }
        /// <summary>
        /// The map zone of the start location in-game.
        /// </summary>
        public MapZone Zone { get; init; }

        /// <summary>
        /// The transition which is used as the initial logical progression for this start location.
        /// </summary>
        public string Transition { get; init; }

        /// <summary>
        /// Returns a sequence of term values which will be treated as setters by the ProgressionInitializer.
        /// <br/>State-valued terms in the sequence will be linked to Start_State, regardless of the int parameter.
        /// </summary>
        public virtual IEnumerable<TermValue> GetStartLocationProgression(LogicManager lm)
        {
            yield return new(lm.GetTermStrict(Transition), 1);
        }

        public virtual ItemChanger.StartDef ToItemChangerStartDef()
        {
            return new ItemChanger.StartDef
            {
                SceneName = SceneName,
                X = X,
                Y = Y,
                MapZone = (int)Zone,
                RespawnFacingRight = true,
                SpecialEffects = ItemChanger.SpecialStartEffects.Default | ItemChanger.SpecialStartEffects.SlowSoulRefill,
            };
        }
    }
}
