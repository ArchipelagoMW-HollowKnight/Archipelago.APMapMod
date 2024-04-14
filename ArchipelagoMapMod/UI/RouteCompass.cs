using ArchipelagoMapMod.Modes;
using ArchipelagoMapMod.Pathfinder;
using MapChanger;
using MapChanger.Defs;
using MapChanger.MonoBehaviours;
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

        if (Knight == null || GameManager.instance.IsNonGameplayScene())
        {
            return;
        }

        Make();
        var arrow = new EmbeddedSprite("GUI.Arrow").Value;

        goCompass.SetActive(false);

        if (RouteManager.CurrentRoute is not null &&
            RouteManager.CurrentRoute.RemainingInstructions.First().TryGetCompassGO(out var go))
        {
            Compass.Locations = new Dictionary<string, CompassLocation>
            {
                ["arrow"] = new TransitionCompassLocation(go, arrow, APmmColors.GetColor(APmmColorSetting.UI_Compass))
            };
            goCompass.SetActive(true);
        }
    }

    private static void Make()
    {
        goCompass = DirectionalCompass.Create
        (
            name: "Route Compass",
            getEntity: () => Knight,
            radius: 1.5f,
            scale: 2.0f,
            condition: IsCompassEnabled,
            rotateSprite: true,
            lerp: false,
            lerpDuration: 0.5f
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