using Archipelago.HollowKnight;
using ArchipelagoMapMod.Settings;
using ArchipelagoMapMod.UI;
using MapChanger;
using MapChanger.Defs;
using Modding;
using RandoMapCore;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace ArchipelagoMapMod;

public class ArchipelagoMapMod : Mod
{
    internal const string MOD = "ArchipelagoMapMod";

    private static readonly string[] dependencies =
    {
        "Archipelago",
        "RandoMapCoreMod",
    };

    internal static ArchipelagoMapMod Instance;

    public bool IsInApSave = false;

    public ArchipelagoMapMod()
    {
        Instance = this;
    }

    internal static Assembly Assembly => Assembly.GetExecutingAssembly();

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

        foreach (var dependency in dependencies)
        {
            if (ModHooks.GetMod(dependency) is not Mod)
            {
                LogWarn($"Dependency not found for {GetType().Name}: {dependency}");
                return;
            }
        }

        Interop.FindInteropMods();
        Finder.InjectLocations(
            JsonUtil.DeserializeFromAssembly<Dictionary<string, MapLocationDef>>(Assembly,
                "ArchipelagoMapMod.Resources.locations.json"));

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
}