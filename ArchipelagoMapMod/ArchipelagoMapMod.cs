using System.Reflection;
using ArchipelagoMapMod.Modes;
using ArchipelagoMapMod.Pathfinder;
using ArchipelagoMapMod.Pathfinder.Instructions;
using ArchipelagoMapMod.Pins;
using ArchipelagoMapMod.RC;
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

public class ArchipelagoMapMod : Mod, ILocalSettings<LocalSettings>, IGlobalSettings<GlobalSettings>, ICustomMenuMod
{
    internal const string MOD = "ArchipelagoMapMod";

    private static readonly string[] dependencies =
    {
        "MapChangerMod",
        "Archipelago",
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
        new APmmColors(),
        new APLogicSetup(),
        new TransitionData(),
        new APmmPathfinder(),
        new APmmPinManager(),
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
        return "2.0.0";
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
        APmmSearchData.LoadConditionalTerms();
        Instruction.LoadCompssObjOverrides();
        InstructionData.LoadWaypointInstructions();
        APmmRoomManager.Load();
        APmmPinManager.Load();
        Finder.InjectLocations(
            JsonUtil.DeserializeFromAssembly<Dictionary<string, MapLocationDef>>(Assembly,
                "ArchipelagoMapMod.Resources.locations.json"));

        Archipelago.HollowKnight.Archipelago.OnArchipelagoGameStarted += OnEnterGame;
        Archipelago.HollowKnight.Archipelago.OnArchipelagoGameEnded += OnQuitToMenu;

        LogDebug("Initialization complete.");
    }

    private static void OnEnterGame()
    {

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
            APmmRoomManager.Make(goMap);
            APmmPinManager.Make(goMap);

            LS.Initialize();

            // Construct pause menu
            title.Make();
            
            // Construct Hint Display
            HintDisplay.Make();

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
        Events.OnSetGameMap -= OnSetGameMap;

        foreach (var hookModule in hookModules) hookModule.OnQuitToMenu();
        
        HintDisplay.Destroy();
    }
    
    public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates)
    {
        return BetterMenu.GetMenuScreen(modListMenu, toggleDelegates);
    }

    public bool ToggleButtonInsideMenu => false;
}