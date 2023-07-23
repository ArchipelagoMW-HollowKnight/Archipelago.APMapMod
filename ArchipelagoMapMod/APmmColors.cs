using GlobalEnums;
using MapChanger;
using UnityEngine;

namespace ArchipelagoMapMod;

internal enum apmmColorSetting
{
    None,

    UI_On,
    UI_Neutral,
    UI_Custom,
    UI_Disabled,
    UI_Special,
    UI_Borders,

    UI_Compass,

    Pin_Normal,
    Pin_Previewed,
    Pin_Out_of_logic,
    Pin_Persistent,
    Pin_Cleared,

    Map_Ancient_Basin,
    Map_City_of_Tears,
    Map_Crystal_Peak,
    Map_Deepnest,
    Map_Dirtmouth,
    Map_Fog_Canyon,
    Map_Forgotten_Crossroads,
    Map_Fungal_Wastes,
    Map_Godhome,
    Map_Greenpath,
    Map_Howling_Cliffs,
    Map_Kingdoms_Edge,
    Map_Queens_Gardens,
    Map_Resting_Grounds,
    Map_Royal_Waterways,
    Map_White_Palace,

    Map_Abyss,
    Map_Hive,
    Map_Ismas_Grove,
    Map_Mantis_Village,
    Map_Queens_Station,
    Map_Soul_Sanctum,
    Map_Watchers_Spire,

    Room_Normal,
    Room_Current,
    Room_Adjacent,
    Room_Out_of_logic,
    Room_Selected,
    Room_Highlighted,
    Room_Debug
}

internal class apmmColors : HookModule
{
    internal static readonly apmmColorSetting[] PinColors =
    {
        apmmColorSetting.Pin_Normal,
        apmmColorSetting.Pin_Previewed,
        apmmColorSetting.Pin_Out_of_logic,
        apmmColorSetting.Pin_Persistent,
        apmmColorSetting.Pin_Cleared
    };

    internal static readonly apmmColorSetting[] RoomColors =
    {
        apmmColorSetting.Room_Normal,
        apmmColorSetting.Room_Current,
        apmmColorSetting.Room_Adjacent,
        apmmColorSetting.Room_Out_of_logic,
        apmmColorSetting.Room_Selected
    };

    private static Dictionary<apmmColorSetting, Vector4> customColors = new();

    private static readonly Dictionary<apmmColorSetting, Vector4> defaultColors = new()
    {
        {apmmColorSetting.Pin_Normal, Color.white},
        {apmmColorSetting.Pin_Previewed, Color.green},
        {apmmColorSetting.Pin_Out_of_logic, Color.red},
        {apmmColorSetting.Pin_Persistent, Color.cyan},
        {apmmColorSetting.Pin_Cleared, Color.magenta},
        {apmmColorSetting.Room_Normal, new Vector4(1f, 1f, 1f, 0.3f)}, // white
        {apmmColorSetting.Room_Current, new Vector4(0, 1f, 0, 0.4f)}, // green
        {apmmColorSetting.Room_Adjacent, new Vector4(0, 1f, 1f, 0.4f)}, // cyan
        {apmmColorSetting.Room_Out_of_logic, new Vector4(1f, 0, 0, 0.3f)}, // red
        {apmmColorSetting.Room_Selected, new Vector4(1f, 1f, 0, 0.7f)}, // yellow
        {apmmColorSetting.Room_Highlighted, new Vector4(1f, 1f, 0.2f, 1f)}, // yellow
        {apmmColorSetting.Room_Debug, new Vector4(0, 0, 1f, 0.5f)}, // blue
        {apmmColorSetting.UI_Compass, new Vector4(1f, 1f, 1f, 0.83f)}
    };

    internal static bool HasCustomColors { get; private set; }

    public override void OnEnterGame()
    {
        Dictionary<string, float[]> customColorsRaw;

        try
        {
            customColorsRaw = JsonUtil.DeserializeFromExternalFile<Dictionary<string, float[]>>(
                Path.Combine(Path.GetDirectoryName(ArchipelagoMapMod.Assembly.Location), "colors.json"));
        }
        catch (Exception)
        {
            ArchipelagoMapMod.Instance.LogError("Invalid colors.json file. Using default colors");
            return;
        }

        if (customColorsRaw is not null)
        {
            foreach (var colorSettingRaw in customColorsRaw.Keys)
            {
                if (!Enum.TryParse(colorSettingRaw, out apmmColorSetting colorSetting)) continue;

                if (customColors.ContainsKey(colorSetting)) continue;

                var rgba = customColorsRaw[colorSettingRaw];

                if (rgba is null || rgba.Length < 4) continue;

                Vector4 color = new(rgba[0] / 256f, rgba[1] / 256f, rgba[2] / 256f, rgba[3]);

                customColors.Add(colorSetting, color);
            }

            MapChangerMod.Instance.Log("Custom colors loaded");
            HasCustomColors = true;
        }
        else
        {
            MapChangerMod.Instance.Log("No colors.json found. Using default colors");
        }
    }

    public override void OnQuitToMenu()
    {
        customColors = new Dictionary<apmmColorSetting, Vector4>();
    }

    internal static Vector4 GetColor(apmmColorSetting apmmColor)
    {
        if (customColors is not null && customColors.ContainsKey(apmmColor)) return customColors[apmmColor];

        if (defaultColors.ContainsKey(apmmColor)) return defaultColors[apmmColor];

        if (Enum.TryParse(apmmColor.ToString(), out ColorSetting mcColor)) return Colors.GetColor(mcColor);

        return Vector4.negativeInfinity;
    }

    internal static Vector4 GetColor(ColorSetting mcColor)
    {
        if (Enum.TryParse(mcColor.ToString(), out apmmColorSetting apmmColor)) return GetColor(apmmColor);

        return Vector4.negativeInfinity;
    }

    internal static Vector4 GetColorFromMapZone(MapZone mapZone)
    {
        return mapZone switch
        {
            MapZone.ABYSS => GetColor(apmmColorSetting.Map_Ancient_Basin),
            MapZone.CITY => GetColor(apmmColorSetting.Map_City_of_Tears),
            MapZone.CLIFFS => GetColor(apmmColorSetting.Map_Howling_Cliffs),
            MapZone.CROSSROADS => GetColor(apmmColorSetting.Map_Forgotten_Crossroads),
            MapZone.MINES => GetColor(apmmColorSetting.Map_Crystal_Peak),
            MapZone.DEEPNEST => GetColor(apmmColorSetting.Map_Deepnest),
            MapZone.TOWN => GetColor(apmmColorSetting.Map_Dirtmouth),
            MapZone.FOG_CANYON => GetColor(apmmColorSetting.Map_Fog_Canyon),
            MapZone.WASTES => GetColor(apmmColorSetting.Map_Fungal_Wastes),
            MapZone.GREEN_PATH => GetColor(apmmColorSetting.Map_Greenpath),
            MapZone.OUTSKIRTS => GetColor(apmmColorSetting.Map_Kingdoms_Edge),
            MapZone.ROYAL_GARDENS => GetColor(apmmColorSetting.Map_Queens_Gardens),
            MapZone.RESTING_GROUNDS => GetColor(apmmColorSetting.Map_Resting_Grounds),
            MapZone.WATERWAYS => GetColor(apmmColorSetting.Map_Royal_Waterways),
            MapZone.WHITE_PALACE => GetColor(apmmColorSetting.Map_White_Palace),
            MapZone.GODS_GLORY => GetColor(apmmColorSetting.Map_Godhome),
            _ => Color.white
        };
    }
}