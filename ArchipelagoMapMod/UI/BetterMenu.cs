using Modding;
using Satchel.BetterMenus;
using UnityEngine;

namespace ArchipelagoMapMod.UI;

internal static class BetterMenu
{
    private static Menu _menuRef;

    public static MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates)
    {
        ArchipelagoMapMod.Instance.Log("Setting up Satchel Menu");
        _menuRef ??= PrepareMenu();
        return _menuRef.GetMenuScreen(modListMenu);
    }

    private static Menu PrepareMenu()
    {
        return new Menu("Archipelago Map Mod", new Element[]
        {
            new CustomSlider(
                "Gameplay Hints shown",
                b =>
                {
                    //APMapMod.Instance.Log($"Setting gameplay hints to {b} from satchel");
                    ArchipelagoMapMod.GS.gameplayHints = Mathf.RoundToInt(b);
                    HintDisplay.UpdateDisplay();
                },
                () => ArchipelagoMapMod.GS.gameplayHints,
                minValue: 0, maxValue: 20, wholeNumbers: true
            ),
            new CustomSlider(
                "Pause Menu Hints Shown",
                b =>
                {
                    //ArchipelagoMapMod.Instance.Log($"Setting pause menu hints to {b} from satchel");
                    ArchipelagoMapMod.GS.pauseMenuHints = Mathf.RoundToInt(b);
                    HintDisplay.UpdateDisplay(ArchipelagoMapMod.GS.pauseMenuHints);
                },
                () => ArchipelagoMapMod.GS.pauseMenuHints,
                minValue: 0, maxValue: 20, wholeNumbers: true
            ),
            new CustomSlider(
                "Hint Text Size",
                b =>
                {
                    //ArchipelagoMapMod.Instance.Log($"Setting hint size to {b} from satchel");
                    ArchipelagoMapMod.GS.hintFontSize = Mathf.RoundToInt(b);
                    HintDisplay.UpdateDisplay();
                },
                () => ArchipelagoMapMod.GS.hintFontSize,
                minValue: 10, maxValue: 50, wholeNumbers: true
            ),
        });
    }
}