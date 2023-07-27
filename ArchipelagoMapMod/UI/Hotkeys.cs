using ArchipelagoMapMod.Modes;
using ArchipelagoMapMod.Pathfinder;
using ArchipelagoMapMod.Pins;
using ArchipelagoMapMod.Rooms;
using MagicUI.Core;
using MapChanger.UI;
using UnityEngine;

namespace ArchipelagoMapMod.UI;

internal class Hotkeys : MapUILayer
{
    public override void BuildLayout()
    {
        Root.ListenForHotkey(KeyCode.H, () =>
        {
            ArchipelagoMapMod.GS.ToggleControlPanel();
            MapUILayerUpdater.Update();
        }, ModifierKeys.Ctrl);

        Root.ListenForHotkey(KeyCode.K, () =>
        {
            ArchipelagoMapMod.GS.ToggleMapKey();
            MapUILayerUpdater.Update();
        }, ModifierKeys.Ctrl);

        Root.ListenForHotkey(KeyCode.P, () =>
        {
            ArchipelagoMapMod.GS.TogglePinSelection();
            UpdateSelectors();
        }, ModifierKeys.Ctrl);

        if (Interop.HasBenchwarp())
            Root.ListenForHotkey(KeyCode.W, () =>
            {
                ArchipelagoMapMod.GS.ToggleBenchwarpPins();
                APmmPinManager.Update();
                UpdateSelectors();
            }, ModifierKeys.Ctrl, () => MapChanger.Settings.MapModEnabled());

        Root.ListenForHotkey(KeyCode.R, () =>
        {
            ArchipelagoMapMod.GS.ToggleRoomSelection();
            UpdateSelectors();
        }, ModifierKeys.Ctrl, () => Conditions.TransitionRandoModeEnabled());

        Root.ListenForHotkey(KeyCode.S, () =>
        {
            ArchipelagoMapMod.GS.ToggleShowReticle();
            UpdateSelectors();
        }, ModifierKeys.Ctrl, () => MapChanger.Settings.MapModEnabled());

        if (Interop.HasBenchwarp())
            Root.ListenForHotkey(KeyCode.B, () =>
            {
                ArchipelagoMapMod.GS.ToggleAllowBenchWarpSearch();
                RouteManager.ResetRoute();
                MapUILayerUpdater.Update();
                RouteCompass.Update();
            }, ModifierKeys.Ctrl, () => Conditions.TransitionRandoModeEnabled());

        Root.ListenForHotkey(KeyCode.G, () =>
        {
            ArchipelagoMapMod.GS.ToggleRouteTextInGame();
            MapUILayerUpdater.Update();
        }, ModifierKeys.Ctrl, () => MapChanger.Settings.MapModEnabled());

        Root.ListenForHotkey(KeyCode.E, () =>
        {
            ArchipelagoMapMod.GS.ToggleWhenOffRoute();
            MapUILayerUpdater.Update();
        }, ModifierKeys.Ctrl, () => MapChanger.Settings.MapModEnabled());

        Root.ListenForHotkey(KeyCode.C, () =>
        {
            ArchipelagoMapMod.GS.ToggleRouteCompassEnabled();
            MapUILayerUpdater.Update();
        }, ModifierKeys.Ctrl, () => Conditions.TransitionRandoModeEnabled());

        Root.ListenForHotkey(KeyCode.Alpha1, () =>
        {
            ArchipelagoMapMod.LS.ToggleSpoilers();
            UpdatePins();
        }, ModifierKeys.Ctrl);

        Root.ListenForHotkey(KeyCode.Alpha2, () =>
        {
            ArchipelagoMapMod.LS.ToggleRandomized();
            UpdatePins();
        }, ModifierKeys.Ctrl);

        Root.ListenForHotkey(KeyCode.Alpha3, () =>
        {
            ArchipelagoMapMod.LS.ToggleVanilla();
            UpdatePins();
        }, ModifierKeys.Ctrl);

        Root.ListenForHotkey(KeyCode.Alpha4, () =>
        {
            ArchipelagoMapMod.GS.TogglePinStyle();
            UpdatePins();
        }, ModifierKeys.Ctrl);

        Root.ListenForHotkey(KeyCode.Alpha5, () =>
        {
            ArchipelagoMapMod.GS.TogglePinSize();
            UpdatePins();
        }, ModifierKeys.Ctrl);

        Root.ListenForHotkey(KeyCode.D, () => { Debugger.LogMapPosition(); }, ModifierKeys.Ctrl);
    }

    protected override bool Condition()
    {
        return Conditions.ArchipelagoMapModEnabled();
    }

    private void UpdateSelectors()
    {
        APmmPinSelector.Instance.MainUpdate();
        TransitionRoomSelector.Instance.MainUpdate();
        MapUILayerUpdater.Update();
    }

    private void UpdatePins()
    {
        PauseMenu.Update();
        APmmPinManager.Update();
        MapUILayerUpdater.Update();
    }
}