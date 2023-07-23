using MapChanger;

namespace ArchipelagoMapMod.Pathfinder;

public class apmmPathfinder : HookModule
{
    internal static apmmSearchData SD { get; private set; }

    internal static InstructionData ID { get; private set; }

    public override void OnEnterGame()
    {
        SD = new apmmSearchData(RandomizerMod.RandomizerMod.RS.TrackerData.pm);

        ID = new InstructionData(SD);

        SD.UpdateProgression();

        //Testing.LogProgressionData(SD);

        //Testing.DebugActions(SD);

        //Testing.SingleStartDestinationTest(SD);

        //Testing.SceneToSceneTest(SD);
    }

    public override void OnQuitToMenu()
    {
        SD = null;
        ID = null;
    }
}