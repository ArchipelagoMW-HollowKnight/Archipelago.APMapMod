using Archipelago.HollowKnight;
using Modding;
using RandoMapCore;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace ArchipelagoMapMod;

public class ArchipelagoMapMod : Mod, IGlobalSettings<GlobalSettings>, IMenuMod
{
    internal const string MOD = "ArchipelagoMapMod";

    private static readonly string[] dependencies =
    {
        "Archipelago",
        "RandoMapCoreMod",
    };

    internal static ArchipelagoMapMod Instance;

    public bool IsInApSave = false;

    public GlobalSettings GS { get; private set; } = new();

    public ArchipelagoMapMod()
    {
        Instance = this;
    }

    internal static Assembly Assembly => Assembly.GetExecutingAssembly();

    public bool ToggleButtonInsideMenu => false;

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

    public override void Initialize()
    {
        Log($"Initializing APMapMod {GetVersion()}");

        foreach (string dependency in dependencies)
        {
            if (ModHooks.GetMod(dependency) is not Mod)
            {
                LogWarn($"Dependency not found for {GetType().Name}: {dependency}");
                return;
            }
        }

        Interop.FindInteropMods();

        ArchipelagoMod.OnArchipelagoGameStarted += OnEnterGame;
        ArchipelagoMod.OnArchipelagoGameEnded += OnQuitToMenu;

        RandoMapCoreMod.AddDataModule(ApmmDataModule.Instance);

        Log("Initialization complete.");
    }

    private void OnEnterGame()
    {
        IsInApSave = true;
    }

    private void OnQuitToMenu()
    {
        IsInApSave = false;
    }

    public void OnLoadGlobal(GlobalSettings s) => GS = s;

    public GlobalSettings OnSaveGlobal() => GS;

    public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
    {
        return [
            new IMenuMod.MenuEntry("Enable Tracker Log",
                ["Off", "On"],
                "Enables logging tracker history for diagnostic purposes",
                i => GS.EnableTrackerLog = i == 1,
                () => GS.EnableTrackerLog ? 1 : 0)
        ];
    }
}