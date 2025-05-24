using ItemChanger;

namespace ArchipelagoMapMod.IC;

public class APmmPlacementTag : Tag
{
    public static event Action<VisitStateChangedEventArgs> OnRandoPlacementVisitStateChanged;
    public static event Action<AbstractPlacement, APmmPlacementTag> OnLoad;

    public override void Load(object parent)
    {
        TagHandlingProperties = TagHandlingFlags.AllowDeserializationFailure;
        ((AbstractPlacement)parent).OnVisitStateChanged += OnVisitStateChanged;
        try
        {
            OnLoad?.Invoke((AbstractPlacement)parent, this);
        }
        catch (Exception e)
        {
            ArchipelagoMapMod.Instance.LogError($"Error invoking RandoPlacementTag.OnLoad:\n{e}");
        }
    }

    public override void Unload(object parent)
    {
        ((AbstractPlacement)parent).OnVisitStateChanged -= OnVisitStateChanged;
    }

    private void OnVisitStateChanged(VisitStateChangedEventArgs args)
    {
        OnRandoPlacementVisitStateChanged?.Invoke(args);
    }
}