using Archipelago.HollowKnight;
using AP = Archipelago.HollowKnight.ArchipelagoMod;

namespace ArchipelagoMapMod.Settings;

public class TransitionSettings
{
    public enum TransitionMode
    {
        None,
        MapAreaRandomizer,
        FullAreaRandomizer,
        RoomRandomizer,
    }
    public TransitionMode Mode = AP.Instance.SlotData.Options.EntranceRandoType switch
    {
        EntranceRandoType.None => TransitionMode.None,
        EntranceRandoType.MapArea => TransitionMode.MapAreaRandomizer,
        EntranceRandoType.FullArea => TransitionMode.FullAreaRandomizer,
        _ => TransitionMode.RoomRandomizer,
    };

    public bool Coupled = AP.Instance.SlotData.Options.ShuffleEntrancesMode == ShuffleEntrancesMode.Coupled;
}