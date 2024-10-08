﻿using Archipelago.HollowKnight;
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
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using Logger = Modding.Logger;

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

    private static readonly List<HookModule> hookModules =
    [
        new APmmColors(),
        new APLogicSetup(),
        new TransitionData(),
        new APmmPathfinder(),
        new APmmPinManager(),
        new TransitionTracker(),
        new DreamgateTracker(),
        new RouteManager(),
        new RouteCompass()
    ];

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

    /// <summary>
    /// Mod version as reported to the modding API
    /// </summary>
    public override string GetVersion()
    {
        Version assemblyVersion = GetType().Assembly.GetName().Version;
        string version = $"{assemblyVersion.Major}.{assemblyVersion.Minor}.{assemblyVersion.Build}";
#if DEBUG
        using SHA1 sha = SHA1.Create();
        using FileStream str = File.OpenRead(GetType().Assembly.Location);
        StringBuilder sb = new();
        foreach (byte b in sha.ComputeHash(str).Take(4))
        {
            sb.AppendFormat("{0:x2}", b);
        }
        version += "-prerelease+" + sb;
#endif
        return version;
    }

    public override int LoadPriority()
    {
        return 10;
    }

    
    // if built with debug flag log all debug message to info
    public new void LogDebug(string message)
    {
#if DEBUG
        Logger.Log($"[Debug] {message}");
#else
        Logger.LogDebug(message);
#endif
        //LogManager.Append(line + Environment.NewLine, logFileName);
    
    }

    public override void Initialize()
    {
        
        Log($"Initializing APMapMod {GetVersion()}");

        foreach (var dependency in dependencies)
        {
            if (ModHooks.GetMod(dependency) is not Mod)
            {
                LogWarn($"Dependency not found for {GetType().Name}: {dependency}");
                return;
            }
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

        ArchipelagoMod.OnArchipelagoGameStarted += OnEnterGame;
        ArchipelagoMod.OnArchipelagoGameEnded += OnQuitToMenu;

        Log("Initialization complete.");
    }

    private static void OnEnterGame()
    {

        MapChanger.Settings.AddModes(modes);
        Events.OnSetGameMap += OnSetGameMap;

        if (Interop.HasBenchwarp())
        {
            BenchwarpInterop.Load();
        }

        foreach (var hookModule in hookModules)
        {
            hookModule.OnEnterGame();
        }
    }

    private static void OnSetGameMap(GameObject goMap)
    {
        try
        {
            // Make rooms and pins
            APmmRoomManager.Make(goMap);
            APmmPinManager.Make(goMap);
            APmmPinManager.SubscribeHints();

            LS.Initialize();

            // Construct pause menu
            title.Make();

            // Construct Hint Display
            try
            {
                HintDisplay.Make();
            }
            catch (Exception ex)
            {
                Instance.LogError("Hint display threw up again, saving the day!");
                Instance.LogError(ex);
            }

            foreach (var button in mainButtons)
            {
                button.Make();
            }

            foreach (var ebp in extraButtonPanels)
            {
                ebp.Make();
            }

            // Construct map UI
            foreach (var uiLayer in mapUILayers)
            {
                MapUILayerUpdater.Add(uiLayer);
            }
        }
        catch (Exception e)
        {
            Instance.LogError(e);
        }
    }

    private static void OnQuitToMenu()
    {
        Events.OnSetGameMap -= OnSetGameMap;

        foreach (var hookModule in hookModules)
        {
            hookModule.OnQuitToMenu();
        }

        HintDisplay.Destroy();
        APmmPinManager.UnsubscribeHints();
    }
    
    public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates)
    {
        return BetterMenu.GetMenuScreen(modListMenu, toggleDelegates);
    }

    public bool ToggleButtonInsideMenu => false;
}