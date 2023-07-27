using ArchipelagoMapMod.Modes;
using ArchipelagoMapMod.Pathfinder;
using MapChanger;
using RandoMapMod.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace ArchipelagoMapMod.UI;

internal class RouteCompass : HookModule
{
    private static GameObject goCompass;
    private static DirectionalCompass Compass => goCompass?.GetComponent<DirectionalCompass>();
    private static GameObject Knight => HeroController.instance?.gameObject;

    public override void OnEnterGame()
    {
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += AfterSceneChange;
    }

    public override void OnQuitToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= AfterSceneChange;
    }

    private static void AfterSceneChange(Scene from, Scene to)
    {
        Update();
    }

    internal static void Update()
    {
        //ArchipelagoMapMod.Instance.LogDebug("Update compass");

        Destroy();

        if (Knight == null || GameManager.instance.IsNonGameplayScene()) return;

        Make();

        goCompass.SetActive(false);

        if (RouteManager.CurrentRoute is not null &&
            RouteManager.CurrentRoute.RemainingInstructions.First().TryGetCompassGO(out var go))
        {
            Compass.TrackedObjects = new List<GameObject> {go};
            goCompass.SetActive(true);
        }
    }

    private static void Make()
    {
        var arrow = new EmbeddedSprite("GUI.Arrow").Value;

        goCompass = DirectionalCompass.Create
        (
            "Route Compass", // name
            Knight, // parent entity
            arrow, // sprite
            APmmColors.GetColor(APmmColorSetting.UI_Compass), // color
            1.5f, // radius
            2.0f, // scale
            IsCompassEnabled, // bool condition
            false, // lerp
            0.5f // lerp duration
        );
    }

    private static void Destroy()
    {
        Object.Destroy(goCompass);
    }

    private static bool IsCompassEnabled()
    {
        return MapChanger.Settings.MapModEnabled()
               && MapChanger.Settings.CurrentMode().GetType().IsSubclassOf(typeof(TransitionRandoMode))
               && ArchipelagoMapMod.GS.ShowRouteCompass;
    }
}