using System.Reflection;
using ArchipelagoMapMod.Modes;
using ArchipelagoMapMod.Pathfinder;
using ArchipelagoMapMod.Pathfinder.Instructions;
using ArchipelagoMapMod.Pins;
using ArchipelagoMapMod.Rooms;
using ArchipelagoMapMod.Settings;
using ArchipelagoMapMod.Transition;
using ArchipelagoMapMod.UI;
using MapChanger;
using MapChanger.Defs;
using MapChanger.UI;
using Modding;
using UnityEngine;

namespace ArchipelagoMapMod;

public class ArchipelagoMapMod : Mod, ILocalSettings<LocalSettings>, IGlobalSettings<GlobalSettings>
{
    internal const string MOD = "ArchipelagoMapMod";

    private static readonly string[] dependencies =
    {
        "MapChangerMod",
        "Randomizer 4",
        "CMICore"
    };

    private static readonly MapMode[] modes =
    {
        new FullMapMode(),
        new AllPinsMode(),
        new PinsOverAreaMode(),
        new PinsOverRoomMode(),
        new TransitionNormalMode(),
        new TransitionVisitedOnlyMode(),
        new TransitionAllRoomsMode()
    };

    private static readonly Title title = new apmmTitle();

    private static readonly MainButton[] mainButtons =
    {
        new ModEnabledButton(),
        new ModeButton(),
        new PinSizeButton(),
        new PinStyleButton(),
        new RandomizedButton(),
        new VanillaButton(),
        new SpoilersButton(),
        new PoolOptionsPanelButton(),
        new PinOptionsPanelButton(),
        new MiscOptionsPanelButton()
    };

    private static readonly ExtraButtonPanel[] extraButtonPanels =
    {
        new PoolOptionsPanel(),
        new PinOptionsPanel(),
        new MiscOptionsPanel()
    };

    private static readonly MapUILayer[] mapUILayers =
    {
        new Hotkeys(),
        new ControlPanel(),
        new MapKey(),
        new SelectionPanels(),
        new apmmBottomRowText(),
        new RouteSummaryText(),
        new RouteText(),
        new QuickMapTransitions()
    };

    private static readonly List<HookModule> hookModules = new()
    {
        new apmmColors(),
        new TransitionData(),
        new apmmPathfinder(),
        new apmmPinManager(),
        new TransitionTracker(),
        new DreamgateTracker(),
        new RouteManager(),
        new RouteCompass()
    };

    internal static ArchipelagoMapMod Instance;

    public static LocalSettings LS = new();

    public static GlobalSettings GS = new();

    public ArchipelagoMapMod()
    {
        Instance = this;
    }

    internal static Assembly Assembly => Assembly.GetExecutingAssembly();

    public void OnLoadGlobal(GlobalSettings gs)
    {
        GS = gs;
    }

    public GlobalSettings OnSaveGlobal()
    {
        return GS;
    }

    public void OnLoadLocal(LocalSettings ls)
    {
        LS = ls;
    }

    public LocalSettings OnSaveLocal()
    {
        return LS;
    }

    public override string GetVersion()
    {
        return "3.4.0";
    }

    public override int LoadPriority()
    {
        return 10;
    }

    public override void Initialize()
    {
        LogDebug("Initializing");

        foreach (var dependency in dependencies)
            if (ModHooks.GetMod(dependency) is not Mod)
            {
                MapChangerMod.Instance.LogWarn($"Dependency not found for {GetType().Name}: {dependency}");
                return;
            }

        Interop.FindInteropMods();
        apmmSearchData.LoadConditionalTerms();
        Instruction.LoadCompssObjOverrides();
        InstructionData.LoadWaypointInstructions();
        apmmRoomManager.Load();
        apmmPinManager.Load();
        Finder.InjectLocations(
            JsonUtil.DeserializeFromAssembly<Dictionary<string, MapLocationDef>>(Assembly,
                "ArchipelagoMapMod.Resources.locations.json"));

        Events.OnEnterGame += OnEnterGame;
        Events.OnQuitToMenu += OnQuitToMenu;

        LogDebug("Initialization complete.");
    }

    private static void OnEnterGame()
    {
        if (!RandomizerMod.RandomizerMod.IsRandoSave) return;

        MapChanger.Settings.AddModes(modes);
        Events.OnSetGameMap += OnSetGameMap;

        if (Interop.HasBenchwarp()) BenchwarpInterop.Load();

        foreach (var hookModule in hookModules) hookModule.OnEnterGame();
    }

    private static void OnSetGameMap(GameObject goMap)
    {
        try
        {
            // Make rooms and pins
            apmmRoomManager.Make(goMap);
            apmmPinManager.Make(goMap);

            LS.Initialize();

            // Construct pause menu
            title.Make();

            foreach (var button in mainButtons) button.Make();

            foreach (var ebp in extraButtonPanels) ebp.Make();

            // Construct map UI
            foreach (var uiLayer in mapUILayers) MapUILayerUpdater.Add(uiLayer);
        }
        catch (Exception e)
        {
            Instance.LogError(e);
        }
    }

    private static void OnQuitToMenu()
    {
        if (!RandomizerMod.RandomizerMod.IsRandoSave) return;

        Events.OnSetGameMap -= OnSetGameMap;

        foreach (var hookModule in hookModules) hookModule.OnQuitToMenu();
    }
}