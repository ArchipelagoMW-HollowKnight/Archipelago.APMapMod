using MapChanger.MonoBehaviours;

namespace ArchipelagoMapMod.Rooms;

internal abstract class RoomSelector : Selector
{
    public override float SelectionRadius { get; } = 2.5f;

    public override float SpriteSize { get; } = 0.6f;

    internal virtual void Initialize(IEnumerable<MapObject> rooms)
    {
        base.Initialize();

        ActiveModifiers.AddRange(new[]
        {
            ActiveByCurrentMode,
            ActiveByToggle
        });

        foreach (var room in rooms)
        {
            var sceneName = "";
            if (room is RoomSprite roomSprite) sceneName = roomSprite.Rsd.SceneName;
            if (room is RoomText roomText) sceneName = roomText.Rtd.Name;

            if (Objects.TryGetValue(sceneName, out var selectables))
                selectables.Add((ISelectable) room);
            else
                Objects[sceneName] = new List<ISelectable> {(ISelectable) room};
        }
    }

    private protected abstract bool ActiveByCurrentMode();
    private protected abstract bool ActiveByToggle();

    public override void OnMainUpdate(bool active)
    {
        base.OnMainUpdate(active);

        SpriteObject.SetActive(ArchipelagoMapMod.GS.ShowReticle);
    }

    protected override void Select(ISelectable selectable)
    {
        selectable.Selected = true;
    }

    protected override void Deselect(ISelectable selectable)
    {
        selectable.Selected = false;
    }
}