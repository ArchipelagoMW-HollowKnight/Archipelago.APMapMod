using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using ArchipelagoMapMod.Modes;
using ArchipelagoMapMod.Rooms;
using ArchipelagoMapMod.UI;
using InControl;
using MapChanger;
using MapChanger.MonoBehaviours;
using UnityEngine;

namespace ArchipelagoMapMod.Pins;

internal class apmmPinSelector : Selector
{
    private const int HIGHLIGHT_HALF_PERIOD = 25;
    private const int HIGHLIGHT_PERIOD = HIGHLIGHT_HALF_PERIOD * 2;

    private static int highlightAnimationTick;

    private readonly Stopwatch attackHoldTimer = new();

    private Coroutine animateHighlightedRooms;
    internal static apmmPinSelector Instance { get; private set; }

    internal static HashSet<ISelectable> HighlightedRooms { get; private set; } = new();

    internal static bool ShowHint { get; private set; }

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used by Unity")]
    private void Update()
    {
        // Press dream nail to toggle lock selection
        if (InputHandler.Instance.inputActions.dreamNail.WasPressed)
        {
            ToggleLockSelection();
            SelectionPanels.UpdatePinPanel();
        }

        // Press quick cast for location hint
        if (InputHandler.Instance.inputActions.quickCast.WasPressed)
            if (!ShowHint)
            {
                ShowHint = true;
                SelectionPanels.UpdatePinPanel();
            }

        // Hold attack to benchwarp
        if (InputHandler.Instance.inputActions.attack.WasPressed) attackHoldTimer.Restart();

        if (InputHandler.Instance.inputActions.attack.WasReleased) attackHoldTimer.Reset();

        if (attackHoldTimer.ElapsedMilliseconds >= 500)
        {
            attackHoldTimer.Reset();

            if (BenchSelected()) GameManager.instance.StartCoroutine(BenchwarpInterop.DoBenchwarp(SelectedObjectKey));
        }
    }

    internal void Initialize(IEnumerable<apmmPin> pins)
    {
        Instance = this;

        base.Initialize();

        ActiveModifiers.AddRange
        (
            new[]
            {
                ActiveByCurrentMode,
                ActiveByToggle
            }
        );

        foreach (var pin in pins)
            if (Objects.TryGetValue(pin.name, out var selectables))
                selectables.Add(pin);
            else
                Objects[pin.name] = new List<ISelectable> {pin};
    }

    public override void OnMainUpdate(bool active)
    {
        base.OnMainUpdate(active);

        SpriteObject.SetActive(ArchipelagoMapMod.GS.ShowReticle);

        if (active)
            StartAnimateHighlightedRooms();
        else
            StopAnimateHighlightedRooms();
    }

    protected override void Select(ISelectable selectable)
    {
        if (selectable is apmmPin pin)
        {
            //ArchipelagoMapMod.Instance.LogDebug($"Selected {pin.name}");
            pin.Selected = true;

            if (pin is RandomizedapmmPin randoPin) SetHighlightedRooms(randoPin);
        }

        static void SetHighlightedRooms(RandomizedapmmPin randoPin)
        {
            if (randoPin.HighlightRooms is null) return;

            HighlightedRooms = new HashSet<ISelectable>(randoPin.HighlightRooms);
        }
    }

    protected override void Deselect(ISelectable selectable)
    {
        if (selectable is apmmPin pin)
            //ArchipelagoMapMod.Instance.LogDebug($"Deselected {pin.name}");
            pin.Selected = false;

        foreach (var room in HighlightedRooms)
        {
            if (room is RoomSprite sprite) sprite.UpdateColor();

            if (room is RoomText text) text.UpdateColor();
        }

        HighlightedRooms.Clear();
    }

    private IEnumerator AnimateHighlightedRooms()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(UpdateWaitSeconds);

            highlightAnimationTick = (highlightAnimationTick + 1) % HIGHLIGHT_PERIOD;

            var color = apmmColors.GetColor(apmmColorSetting.Room_Highlighted);
            color.w = 0.3f + TriangleWave(highlightAnimationTick) * 0.7f;

            foreach (var room in HighlightedRooms)
                if (room is RoomSprite roomSprite)
                    roomSprite.Color = color;
                else if (room is RoomText text) text.Color = color;
        }

        static float TriangleWave(float x)
        {
            return Math.Abs(x - HIGHLIGHT_HALF_PERIOD) / HIGHLIGHT_HALF_PERIOD;
        }
    }

    private void StartAnimateHighlightedRooms()
    {
        animateHighlightedRooms ??= StartCoroutine(AnimateHighlightedRooms());
    }

    private void StopAnimateHighlightedRooms()
    {
        if (animateHighlightedRooms is not null)
        {
            StopCoroutine(AnimateHighlightedRooms());
            animateHighlightedRooms = null;
        }
    }

    protected override void OnSelectionChanged()
    {
        ShowHint = false;
        SelectionPanels.UpdatePinPanel();
        SelectionPanels.UpdateRoomPanel();
    }

    private bool ActiveByCurrentMode()
    {
        return Conditions.ArchipelagoMapModEnabled();
    }

    private bool ActiveByToggle()
    {
        return ArchipelagoMapMod.GS.PinSelectionOn;
    }

    internal string GetText()
    {
        if (apmmPinManager.Pins.TryGetValue(SelectedObjectKey, out var pin))
        {
            var text = pin.GetSelectionText();

            List<BindingSource> attackBindings = new(InputHandler.Instance.inputActions.attack.Bindings);

            if (BenchSelected())
                text +=
                    $"\n\nHold {Utils.GetBindingsText(attackBindings)} to benchwarp.";

            List<BindingSource> quickCastBindings = new(InputHandler.Instance.inputActions.quickCast.Bindings);

            if (pin.HintText is not null)
            {
                if (ShowHint)
                    text += pin.HintText;
                else
                    text +=
                        $"\n\nPress {Utils.GetBindingsText(quickCastBindings)} to reveal location hint.";
            }

            List<BindingSource> dreamNailBindings = new(InputHandler.Instance.inputActions.dreamNail.Bindings);

            if (LockSelection)
                text +=
                    $"\n\nPress {Utils.GetBindingsText(dreamNailBindings)} to unlock pin selection.";
            else
                text +=
                    $"\n\nPress {Utils.GetBindingsText(dreamNailBindings)} to lock pin selection.";

            return text;
        }

        return "";
    }

    internal bool BenchSelected()
    {
        return Interop.HasBenchwarp() && ArchipelagoMapMod.GS.ShowBenchwarpPins &&
               BenchwarpInterop.IsVisitedBench(SelectedObjectKey);
    }
}