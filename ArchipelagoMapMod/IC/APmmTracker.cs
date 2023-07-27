using ItemChanger;
using ItemChanger.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UObject = UnityEngine.Object;
using Module = ItemChanger.Modules.Module;
using System.Collections.ObjectModel;

namespace ArchipelagoMapMod.IC;

public class APmmTracker : Module
{
    
    /// <summary>
    /// Event which is invoked during ItemChanger.Events.OnEnterGame, to allow access to the item and placement lookups after all items and placements have loaded.
    /// </summary>
    public static event Action OnLoadComplete;

    /// <summary>
    /// Item lookup indexed parallel to the RandoContext item placement list. Refreshed on entering the game.
    /// </summary>
    public static ReadOnlyDictionary<int, AbstractItem> Items { get; }
    private static readonly Dictionary<int, AbstractItem> _items;

    /// <summary>
    /// Placement lookup indexed parallel to the RandoContext item placement list. Refreshed on entering the game.
    /// </summary>
    public static ReadOnlyDictionary<int, AbstractPlacement> Placements { get; }
    private static readonly Dictionary<int, AbstractPlacement> _placements;

    static APmmTracker() {
        Items = new(_items = new());
        Placements = new(_placements = new());
    }

    public override void Initialize()
    {
        Events.OnEnterGame += InvokeOnLoadComplete;
        APmmItemTag.OnLoad += RecordItem;
        APmmPlacementTag.OnLoad += RecordPlacement;

        _items.Clear();
        _placements.Clear();
    }

    public override void Unload()
    {
        Events.OnEnterGame -= InvokeOnLoadComplete;
        APmmItemTag.OnLoad -= RecordItem;
        APmmPlacementTag.OnLoad -= RecordPlacement;

        _items.Clear();
        _placements.Clear();
    }

    private static void InvokeOnLoadComplete()
    {
        try
        {
            OnLoadComplete?.Invoke();
        }
        catch (Exception e)
        {
            ArchipelagoMapMod.Instance.LogError($"Error invoking RandomizerModule.OnLoadComplete:\n{e}");
        }
    }

    private static void RecordItem(AbstractItem item, APmmItemTag tag)
    {
        _items[tag.id] = item;
    }

    private static void RecordPlacement(AbstractPlacement placement, APmmPlacementTag tag)
    {
        if (tag.ids != null)
        {
            foreach (int id in tag.ids) _placements[id] = placement;
        }
    }
}
