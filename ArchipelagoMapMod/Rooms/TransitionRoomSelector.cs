using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using ArchipelagoMapMod.Modes;
using ArchipelagoMapMod.Pathfinder;
using ArchipelagoMapMod.Pathfinder.Instructions;
using ArchipelagoMapMod.Pins;
using ArchipelagoMapMod.Transition;
using ArchipelagoMapMod.UI;
using InControl;
using MapChanger;
using MapChanger.MonoBehaviours;

namespace ArchipelagoMapMod.Rooms;

internal class TransitionRoomSelector : RoomSelector
{
    internal static TransitionRoomSelector Instance;

    private readonly Stopwatch attackHoldTimer = new();

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used by Unity")]
    private void Update()
    {
        if (InputHandler.Instance.inputActions.menuSubmit.WasPressed
            && SelectedObjectKey is not NONE_SELECTED)
        {
            attackHoldTimer.Reset();

            RouteManager.TryGetNextRouteTo(SelectedObjectKey);

            RouteText.Instance.Update();
            RouteSummaryText.Instance.Update();
            SelectionPanels.UpdateRoomPanel();
        }

        if (InputHandler.Instance.inputActions.attack.WasPressed) attackHoldTimer.Restart();

        if (InputHandler.Instance.inputActions.attack.WasReleased) attackHoldTimer.Reset();

        // Disable this benchwarp if the pin selector has already selected a bench
        if (attackHoldTimer.ElapsedMilliseconds >= 500)
        {
            attackHoldTimer.Reset();

            if (!APmmPinSelector.Instance.BenchSelected()) TryBenchwarp();
        }
    }

    internal override void Initialize(IEnumerable<MapObject> rooms)
    {
        Instance = this;

        base.Initialize(rooms);
    }

    public override void OnMainUpdate(bool active)
    {
        base.OnMainUpdate(active);

        attackHoldTimer.Reset();
    }

    private protected override bool ActiveByCurrentMode()
    {
        return Conditions.TransitionRandoModeEnabled();
    }

    private protected override bool ActiveByToggle()
    {
        return ArchipelagoMapMod.GS.RoomSelectionOn;
    }

    protected override void OnSelectionChanged()
    {
        SelectionPanels.UpdateRoomPanel();
    }

    internal string GetText()
    {
        var instructions = GetInstructionText();
        var transitions = TransitionData.GetUncheckedVisited(SelectedObjectKey);

        if (transitions is "") return instructions;

        return $"{instructions}\n\n{transitions}";
    }

    private static void TryBenchwarp()
    {
        if (Interop.HasBenchwarp() && RouteManager.CurrentRoute is not null &&
            RouteManager.CurrentRoute.RemainingInstructions.First() is StartWarpInstruction or BenchwarpInstruction)
            GameManager.instance.StartCoroutine(
                BenchwarpInterop.DoBenchwarp(RouteManager.CurrentRoute.RemainingInstructions.First().Text));
    }

    private static string GetInstructionText()
    {
        var selectedScene = Instance.SelectedObjectKey;
        var text = "";

        text += $"Selected room: {selectedScene}.";

        List<BindingSource> bindings = new(InputHandler.Instance.inputActions.menuSubmit.Bindings);

        if (selectedScene == Utils.CurrentScene()) text += " You are here.";

        text += $"\n\nPress {Utils.GetBindingsText(bindings)}";

        if (RouteManager.CanCycleRoute(selectedScene))
            text += " to change starting / final transitions of current route.";
        else
            text += " to find a new route.";

        if (!APmmPinSelector.Instance.BenchSelected() && RouteManager.TryGetBenchwarpKey(out var _))
        {
            bindings = new List<BindingSource>(InputHandler.Instance.inputActions.attack.Bindings);

            text += $" Hold {Utils.GetBindingsText(bindings)} to benchwarp.";
        }

        return text;
    }
}