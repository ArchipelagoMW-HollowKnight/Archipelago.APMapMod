using GlobalEnums;
using MapChanger;
using UnityEngine;

namespace ArchipelagoMapMod;

internal enum APmmColorSetting
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

internal class APmmColors : HookModule
{
    internal static readonly APmmColorSetting[] PinColors =
    {
        APmmColorSetting.Pin_Normal,
        APmmColorSetting.Pin_Previewed,
        APmmColorSetting.Pin_Out_of_logic,
        APmmColorSetting.Pin_Persistent,
        APmmColorSetting.Pin_Cleared
    };

    internal static readonly APmmColorSetting[] RoomColors =
    {
        APmmColorSetting.Room_Normal,
        APmmColorSetting.Room_Current,
        APmmColorSetting.Room_Adjacent,
        APmmColorSetting.Room_Out_of_logic,
        APmmColorSetting.Room_Selected
    };

    private static Dictionary<APmmColorSetting, Vector4> customColors = [];

    private static readonly Dictionary<APmmColorSetting, Vector4> defaultColors = new()
    {
        {APmmColorSetting.Pin_Normal, Color.white},
        {APmmColorSetting.Pin_Previewed, Color.green},
        {APmmColorSetting.Pin_Out_of_logic, Color.red},
        {APmmColorSetting.Pin_Persistent, Color.cyan},
        {APmmColorSetting.Pin_Cleared, Color.magenta},
        {APmmColorSetting.Room_Normal, new Vector4(1f, 1f, 1f, 0.3f)}, // white
        {APmmColorSetting.Room_Current, new Vector4(0, 1f, 0, 0.4f)}, // green
        {APmmColorSetting.Room_Adjacent, new Vector4(0, 1f, 1f, 0.4f)}, // cyan
        {APmmColorSetting.Room_Out_of_logic, new Vector4(1f, 0, 0, 0.3f)}, // red
        {APmmColorSetting.Room_Selected, new Vector4(1f, 1f, 0, 0.7f)}, // yellow
        {APmmColorSetting.Room_Highlighted, new Vector4(1f, 1f, 0.2f, 1f)}, // yellow
        {APmmColorSetting.Room_Debug, new Vector4(0, 0, 1f, 0.5f)}, // blue
        {APmmColorSetting.UI_Compass, new Vector4(1f, 1f, 1f, 0.83f)}
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
                if (!Enum.TryParse(colorSettingRaw, out APmmColorSetting colorSetting)) continue;

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
        customColors = [];
    }

    internal static Vector4 GetColor(APmmColorSetting aPmmColor)
    {
        if (customColors is not null && customColors.ContainsKey(aPmmColor)) return customColors[aPmmColor];

        if (defaultColors.ContainsKey(aPmmColor)) return defaultColors[aPmmColor];

        if (Enum.TryParse(aPmmColor.ToString(), out ColorSetting mcColor)) return Colors.GetColor(mcColor);

        return Vector4.negativeInfinity;
    }

    internal static Vector4 GetColor(ColorSetting mcColor)
    {
        if (Enum.TryParse(mcColor.ToString(), out APmmColorSetting apmmColor)) return GetColor(apmmColor);

        return Vector4.negativeInfinity;
    }

    internal static Vector4 GetColorFromMapZone(MapZone mapZone)
    {
        return mapZone switch
        {
            MapZone.ABYSS => GetColor(APmmColorSetting.Map_Ancient_Basin),
            MapZone.CITY => GetColor(APmmColorSetting.Map_City_of_Tears),
            MapZone.CLIFFS => GetColor(APmmColorSetting.Map_Howling_Cliffs),
            MapZone.CROSSROADS => GetColor(APmmColorSetting.Map_Forgotten_Crossroads),
            MapZone.MINES => GetColor(APmmColorSetting.Map_Crystal_Peak),
            MapZone.DEEPNEST => GetColor(APmmColorSetting.Map_Deepnest),
            MapZone.TOWN => GetColor(APmmColorSetting.Map_Dirtmouth),
            MapZone.FOG_CANYON => GetColor(APmmColorSetting.Map_Fog_Canyon),
            MapZone.WASTES => GetColor(APmmColorSetting.Map_Fungal_Wastes),
            MapZone.GREEN_PATH => GetColor(APmmColorSetting.Map_Greenpath),
            MapZone.OUTSKIRTS => GetColor(APmmColorSetting.Map_Kingdoms_Edge),
            MapZone.ROYAL_GARDENS => GetColor(APmmColorSetting.Map_Queens_Gardens),
            MapZone.RESTING_GROUNDS => GetColor(APmmColorSetting.Map_Resting_Grounds),
            MapZone.WATERWAYS => GetColor(APmmColorSetting.Map_Royal_Waterways),
            MapZone.WHITE_PALACE => GetColor(APmmColorSetting.Map_White_Palace),
            MapZone.GODS_GLORY => GetColor(APmmColorSetting.Map_Godhome),
            _ => Color.white
        };
    }
}