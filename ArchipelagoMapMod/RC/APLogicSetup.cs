using ArchipelagoMapMod.IC;
using ArchipelagoMapMod.RandomizerData;
using ArchipelagoMapMod.Settings;
using ItemChanger;
using MapChanger;
using Newtonsoft.Json;

namespace ArchipelagoMapMod.RC;

public class APLogicSetup : HookModule
{

    [JsonIgnore]
    public static APRandoContext Context;
    
    public override void OnEnterGame()
    {
        Data.Load();
        
        ItemChangerMod.Modules.GetOrAdd<APmmTrackerUpdate>();
        
        //TODO: fix start location when AP provides the info.
        Context = new APRandoContext(new GenerationSettings()
        {
            StartLocation = "King's Pass"
        }, Data.Starts["King's Pass"]);
        ArchipelagoMapMod.LS.TrackerData ??= new TrackerData();
        ArchipelagoMapMod.LS.TrackerData.Setup(Context);
    }

    public override void OnQuitToMenu()
    {
    }
}