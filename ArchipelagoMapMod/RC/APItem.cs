using ArchipelagoMapMod.RandomizerData;
using RandomizerCore;
using RandomizerCore.LogicItems;

namespace ArchipelagoMapMod.RC;

public class APItem : RandoItem
{
    // /// <summary>
    // /// The ItemRequestInfo associated with the item. May be null if the item does not require modification.
    // /// <br/>This field is not serialized and will be null upon reloading the game.
    // /// </summary>
    // [JsonIgnore] public ItemRequestInfo? info;
    
    /// <summary>
    /// The ItemDef associated with the location. Preferred over Data.GetItemDef, since this preserves modified item data.
    /// <br/>This field is serialized, and is safe to use after reloading the game. May rarely be null for external items which choose not to supply a value.
    /// </summary>
    public ItemDef ItemDef;

    public APItem(LogicItem logicItem)
    {
        item = logicItem;
    }


    public APItem(string name)
    {
        item = new EmptyItem(name);
    }
}
