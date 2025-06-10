using ArchipelagoMapMod.Settings;
using MapChanger.UI;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ArchipelagoMapMod.UI.SkipOptionsGrid;

internal class SkipOptionsGrid : ExtraButtonGrid
{
    public override int RowSize => 8;

    private readonly IReadOnlyList<(string name, FieldInfo field)> skipToggles;

    public SkipOptionsGrid()
    {
        List<(string name, FieldInfo field)> toggles = [];
        foreach (FieldInfo field in typeof(SkipSettings).GetFields())
        {
            string displayName = Regex.Replace(field.Name, "(?<!^)([A-Z])", " $1");
            toggles.Add((displayName, field));
        }
        skipToggles = toggles;
    }

    protected override IEnumerable<ExtraButton> GetButtons()
    {
        return skipToggles.Select(x => new SkipOptionButton(x.name, x.field));
    }
}
