using Archipelago.HollowKnight.IC;
using ArchipelagoMapMod.IC;
using ArchipelagoMapMod.Pins;
using ArchipelagoMapMod.RandomizerData;
using ArchipelagoMapMod.Settings;
using ItemChanger;
using ItemChanger.Internal;
using MapChanger;
using Newtonsoft.Json;
using RandomizerCore;

namespace ArchipelagoMapMod.RC;

public class APLogicSetup : HookModule
{

    [JsonIgnore]
    public static APRandoContext Context;
    
    public override void OnEnterGame()
    {
        Data.Load();
        
        ItemChangerMod.Modules.Add<APmmTrackerUpdate>();
        
        //TODO: fix start location when AP provides the info.
        Context ??= new APRandoContext(new GenerationSettings(), Data.Starts["King's Pass"]);
        ArchipelagoMapMod.LS.TrackerData ??= new TrackerData {AllowSequenceBreaks = true, logFileName = "TrackerDataDebugHistory.txt"};
        ArchipelagoMapMod.LS.TrackerDataWithoutSequenceBreaks ??= new TrackerData {AllowSequenceBreaks = false, logFileName = "TrackerDataWithoutSequenceBreaksDebugHistory.txt"};
        ArchipelagoMapMod.LS.TrackerDataWithoutSequenceBreaks?.Setup(Context);
        ArchipelagoMapMod.LS.TrackerData?.Setup(Context);
    }

    public override void OnQuitToMenu()
    {
    }
}