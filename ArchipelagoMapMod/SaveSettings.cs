namespace ArchipelagoMapMod;

public class SaveSettings
{
    public Dictionary<string, string> TrackerVisitedTransitionsNoSequenceBreak = [];
    public HashSet<string> TrackerOutOfLogicVisitedTransitions = [];

    public Dictionary<string, string> TrackerVisitedTransitionsWithSequenceBreak = [];
}
