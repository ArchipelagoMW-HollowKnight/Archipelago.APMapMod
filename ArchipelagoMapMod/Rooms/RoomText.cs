using System.Diagnostics.CodeAnalysis;
using ArchipelagoMapMod.Modes;
using ArchipelagoMapMod.Transition;
using MapChanger;
using MapChanger.MonoBehaviours;
using TMPro;
using UnityEngine;

namespace ArchipelagoMapMod.Rooms;

internal class RoomText : MapObject, ISelectable
{
    private TMP_FontAsset font;

    private bool selected;

    private TextMeshPro tmp;
    internal RoomTextDef Rtd { get; private set; }

    internal Vector4 Color
    {
        get => tmp.color;
        set => tmp.color = value;
    }

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used by Unity")]
    private void Start()
    {
        tmp.sortingLayerID = 629535577;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.font = font;
        tmp.fontSize = 2.4f;
        tmp.text = Rtd.Name;
    }

    public bool Selected
    {
        get => selected;
        set
        {
            if (selected != value)
            {
                selected = value;
                UpdateColor();
            }
        }
    }

    public bool CanSelect()
    {
        return gameObject.activeInHierarchy;
    }

    public (string, Vector2) GetKeyAndPosition()
    {
        return (Rtd.Name, transform.position);
    }

    internal void Initialize(RoomTextDef rtd, TMP_FontAsset font)
    {
        Rtd = rtd;
        this.font = font;

        base.Initialize();

        ActiveModifiers.AddRange
        (
            new[]
            {
                Conditions.TransitionRandoModeEnabled,
                ActiveByMap,
                GetRoomActive
            }
        );

        tmp = gameObject.AddComponent<TextMeshPro>();
        transform.localPosition = new Vector3(Rtd.X, Rtd.Y, 0f);
    }

    private bool ActiveByMap()
    {
        return States.WorldMapOpen || (States.QuickMapOpen && States.CurrentMapZone == Rtd.MapZone);
    }

    private bool GetRoomActive()
    {
        return TransitionTracker.GetRoomActive(Rtd.Name);
    }

    public override void OnMainUpdate(bool active)
    {
        UpdateColor();
    }

    internal void UpdateColor()
    {
        if (selected)
            Color = APmmColors.GetColor(APmmColorSetting.Room_Selected);
        else
            Color = TransitionTracker.GetRoomColor(Rtd.Name);
    }
}