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
    //TODO: Point to AP's settings when added to HKAP
    public TransitionMode Mode = TransitionMode.None;

    public enum AreaConstraintSetting
    {
        None,
        MoreConnectedMapAreas,
        MoreConnectedTitledAreas,
    }

    public AreaConstraintSetting AreaConstraint = AreaConstraintSetting.None;

    /*
    // This will likely be difficult to implement -- not many rooms which don't have items or npcs or events
    // and then even fewer combinations which give matching transition counts
    public enum RemoveRoomsSetting
    {
        None,
        RemoveEmptyHallways,
        AggressivelyRemoveRooms,
    }
    public RemoveRoomsSetting Remove;
    */

    public enum TransitionMatchingSetting
    {
        MatchingDirections,
        MatchingDirectionsAndNoDoorToDoor,
        NonmatchingDirections
    }
    public TransitionMatchingSetting TransitionMatching = TransitionMatchingSetting.MatchingDirections;
    public bool Coupled = true;
}