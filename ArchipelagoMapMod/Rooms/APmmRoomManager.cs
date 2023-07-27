using MapChanger;
using MapChanger.Map;
using MapChanger.MonoBehaviours;
using TMPro;
using UnityEngine;

namespace ArchipelagoMapMod.Rooms;

internal class APmmRoomManager
{
    private static Dictionary<string, RoomTextDef> roomTextDefs;

    internal static MapObject MoRoomTexts { get; private set; }

    internal static void Load()
    {
        roomTextDefs = JsonUtil.DeserializeFromAssembly<Dictionary<string, RoomTextDef>>(ArchipelagoMapMod.Assembly,
            "ArchipelagoMapMod.Resources.roomTexts.json");

        if (Dependencies.HasAdditionalMaps())
        {
            var roomTextDefsAM =
                JsonUtil.DeserializeFromAssembly<Dictionary<string, RoomTextDef>>(ArchipelagoMapMod.Assembly,
                    "ArchipelagoMapMod.Resources.roomTextsAM.json");
            foreach ((var scene, var rtd) in roomTextDefsAM.Select(kvp => (kvp.Key, kvp.Value)))
            {
                if (!roomTextDefs.ContainsKey(scene)) continue;
                if (rtd is null)
                    roomTextDefs.Remove(scene);
                else
                    roomTextDefs[scene] = rtd;
            }
        }
    }

    internal static void Make(GameObject goMap)
    {
        MoRoomTexts = Utils.MakeMonoBehaviour<MapObject>(goMap, "Room Texts");
        MoRoomTexts.Initialize();

        var tmpFont = goMap.transform.Find("Cliffs").Find("Area Name (1)").GetComponent<TextMeshPro>().font;

        foreach ((var scene, var rtd) in roomTextDefs.Select(kvp => (kvp.Key, kvp.Value)))
        {
            var roomText = Utils.MakeMonoBehaviour<RoomText>(null, $"Room Text {rtd.Name}");
            roomText.Initialize(rtd, tmpFont);
            MoRoomTexts.AddChild(roomText);
        }

        MapObjectUpdater.Add(MoRoomTexts);

        var transitionRoomSelector =
            Utils.MakeMonoBehaviour<TransitionRoomSelector>(null, "ArchipelagoMapMod Transition Room Selector");
        //transitionRoomSelector.Initialize(BuiltInObjects.MappedRooms.Values.Concat(MoRoomTexts.Children));
    }
}