using ArchipelagoMapMod.Modes;
using ArchipelagoMapMod.Settings;
using ArchipelagoMapMod.Transition;
using GlobalEnums;
using MapChanger;
using MapChanger.MonoBehaviours;
using RandomizerCore.Logic;
using UnityEngine;
//using L = RandomizerMod.Localization;

namespace ArchipelagoMapMod.Pins;

internal abstract class apmmPin : BorderedPin, ISelectable
{
    private const float SMALL_SCALE = 0.56f;
    private const float MEDIUM_SCALE = 0.67f;
    private const float LARGE_SCALE = 0.8f;

    private protected const float UNREACHABLE_SIZE_MULTIPLIER = 0.7f;
    private protected const float UNREACHABLE_COLOR_MULTIPLIER = 0.5f;

    private protected const float SELECTED_MULTIPLIER = 1.3f;

    private protected static readonly Dictionary<PinSize, float> pinSizes = new()
    {
        {PinSize.Small, SMALL_SCALE},
        {PinSize.Medium, MEDIUM_SCALE},
        {PinSize.Large, LARGE_SCALE}
    };

    private protected DNFLogicDef[] hints;

    private bool selected;

    internal string ModSource { get; private protected set; } = $"{char.MaxValue}ArchipelagoMapMod";
    internal string LocationPoolGroup { get; private protected set; }
    internal abstract HashSet<string> ItemPoolGroups { get; }
    internal string SceneName { get; private protected set; }
    internal MapZone MapZone { get; private protected set; } = MapZone.NONE;
    internal LogicDef Logic { get; private set; }
    internal int PinGridIndex { get; private protected set; }
    internal string HintText { get; private set; }

    public virtual bool Selected
    {
        get => selected;
        set
        {
            if (selected != value)
            {
                selected = value;
                UpdatePinSize();
                UpdatePinColor();
                UpdateBorderColor();
            }
        }
    }

    public bool CanSelect()
    {
        return Sr.isVisible;
    }

    public virtual (string, Vector2) GetKeyAndPosition()
    {
        return (name, transform.position);
    }

    public override void Initialize()
    {
        base.Initialize();

        ActiveModifiers.AddRange
        (
            new[]
            {
                CorrectMapOpen,
                ActiveByCurrentMode,
                ActiveBySettings,
                ActiveByProgress
            }
        );

        //if (RandomizerMod.RandomizerMod.RS.TrackerData.lm.LogicLookup.TryGetValue(name, out var logic)) Logic = logic;

        BorderSprite = new EmbeddedSprite("Pins.Border").Value;
        BorderPlacement = BorderPlacement.InFront;
    }

    public override void OnMainUpdate(bool active)
    {
        if (!active) return;

        UpdatePinSprite();
        UpdatePinSize();
        UpdatePinColor();
        UpdateBorderColor();
        UpdateHintText();
    }

    private protected abstract void UpdatePinSprite();

    private protected abstract void UpdatePinSize();

    private protected abstract void UpdatePinColor();

    private protected abstract void UpdateBorderColor();

    private void UpdateHintText()
    {
        if (hints is null || !hints.Any()) return;

        var text = "\n";

        // foreach (var hint in hints)
        //     if (hint.CanGet(RandomizerMod.RandomizerMod.RS.TrackerData.pm))
        //         text += $"\n{hint.Name}";

        HintText = text is not "\n" ? text : null;
    }

    private protected bool CorrectMapOpen()
    {
        return States.WorldMapOpen ||
               (States.QuickMapOpen && (States.CurrentMapZone == MapZone || MapZone is MapZone.NONE));
    }

    private protected bool ActiveByCurrentMode()
    {
        return MapZone is MapZone.NONE
               || MapChanger.Settings.CurrentMode() is FullMapMode or AllPinsMode
               || (MapChanger.Settings.CurrentMode() is PinsOverAreaMode && Utils.HasMapSetting(MapZone))
               || (MapChanger.Settings.CurrentMode() is PinsOverRoomMode && Utils.HasMapSetting(MapZone)
                                                                         && Tracker.HasVisitedScene(
                                                                                  Finder.GetMappedScene(SceneName)) &&
                                                                              (PlayerData.instance.GetBool(
                                                                                   nameof(PlayerData.hasQuill)) ||
                                                                               ArchipelagoMapMod.GS.AlwaysHaveQuill))
               || (Conditions.TransitionRandoModeEnabled() && TransitionTracker.GetRoomActive(SceneName))
               || (Interop.HasBenchwarp() && ArchipelagoMapMod.GS.ShowBenchwarpPins && IsVisitedBench());
    }

    private protected abstract bool ActiveBySettings();

    private protected abstract bool ActiveByProgress();

    internal virtual string GetSelectionText()
    {
        ;
        var text = $"{name.ToCleanName()}";

        //if (SceneName is not null) text += $"\n\n{L.Localize("Room")}: {SceneName}";

        return text;
    }

    internal virtual bool IsVisitedBench()
    {
        return false;
    }
}