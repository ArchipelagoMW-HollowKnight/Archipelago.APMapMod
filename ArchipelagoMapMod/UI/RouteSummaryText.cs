using ArchipelagoMapMod.Modes;
using ArchipelagoMapMod.Pathfinder;
using ArchipelagoMapMod.Pathfinder.Instructions;
using MagicUI.Elements;
using MapChanger;
using MapChanger.UI;
using L = RandomizerMod.Localization;

namespace ArchipelagoMapMod.UI;

internal class RouteSummaryText : MapUILayer
{
    internal static RouteSummaryText Instance;

    private static TextObject routeSummary;

    protected override bool Condition()
    {
        return Conditions.TransitionRandoModeEnabled()
               && States.WorldMapOpen;
    }

    public override void BuildLayout()
    {
        Instance = this;
        routeSummary = UIExtensions.TextFromEdge(Root, "Route Summary", true);
    }

    public override void Update()
    {
        routeSummary.Text = GetSummaryText();
    }

    private static string GetSummaryText()
    {
        var text = $"{L.Localize("Current route")}: ";

        if (RouteManager.CurrentRoute is null) return text += L.Localize("None");

        var first = RouteManager.CurrentRoute.FirstInstruction;
        var last = RouteManager.CurrentRoute.RemainingInstructions.Last();

        if (last is TransitionInstruction ti)
        {
            text += $"{first.Text.ToCleanName()} ->...-> {ti.TargetTransition.ToCleanName()}";
        }
        else
        {
            text += first.Text.ToCleanName();

            if (first != last) text += $" ->...-> {last.Text.ToCleanName()}";
        }

        return text += $"\n\n{L.Localize("Transitions")}: {RouteManager.CurrentRoute.TotalInstructionCount}";
    }
}