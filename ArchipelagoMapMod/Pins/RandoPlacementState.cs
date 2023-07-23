namespace ArchipelagoMapMod.Pins;

internal enum RandoPlacementState
{
    UncheckedUnreachable,
    UncheckedReachable,
    OutOfLogicReachable,
    PreviewedUnreachable,
    PreviewedReachable,
    Cleared,
    ClearedPersistent
}