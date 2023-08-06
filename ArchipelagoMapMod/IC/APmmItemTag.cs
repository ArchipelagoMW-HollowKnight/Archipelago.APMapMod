using ItemChanger;

namespace ArchipelagoMapMod.IC;

public class APmmItemTag : Tag
{
    public static event Action<int, ReadOnlyGiveEventArgs> AfterRandoItemGive;
    public static event Action<AbstractItem, APmmItemTag> OnLoad;

    public int id;
    public bool obtained = false;

    public override void Load(object parent)
    {
        TagHandlingProperties = TagHandlingFlags.AllowDeserializationFailure;
        ((AbstractItem)parent).AfterGive += Broadcast;
        try
        {
            OnLoad?.Invoke((AbstractItem)parent, this);
        }
        catch (Exception e)
        {
            ArchipelagoMapMod.Instance.LogError($"Error invoking RandoItemTag.OnLoad:\n{e}");
        }
    }

    public override void Unload(object parent)
    {
        ((AbstractItem)parent).AfterGive -= Broadcast;
    }

    private void Broadcast(ReadOnlyGiveEventArgs args)
    {
        if (!obtained) AfterRandoItemGive?.Invoke(id, args);
        obtained = true;
    }
}