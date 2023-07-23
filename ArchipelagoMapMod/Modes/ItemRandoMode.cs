using ArchipelagoMapMod.Settings;
using ArchipelagoMapMod.Transition;
using MapChanger;
using MapChanger.Defs;
using MapChanger.MonoBehaviours;
using UnityEngine;

namespace ArchipelagoMapMod.Modes;

internal class ItemRandoMode : apmmMapMode
{
    public override bool DisableAreaNames => !ArchipelagoMapMod.GS.ShowAreaNames;

    public override bool InitializeToThis()
    {
        if (TransitionData.IsTransitionRando()) return false;

        return ModeName == ArchipelagoMapMod.GS.DefaultItemRandoMode.ToString().ToCleanName();
    }

    public override Vector4? RoomColorOverride(RoomSprite roomSprite)
    {
        if (roomSprite.Selected) return apmmColors.GetColor(apmmColorSetting.Room_Highlighted);

        return GetCustomColor(roomSprite.Rsd.ColorSetting);
    }

    public override Vector4? AreaNameColorOverride(AreaName areaName)
    {
        return GetCustomColor(areaName.MiscObjectDef.ColorSetting);
    }

    public override bool? NextAreaNameActiveOverride(NextAreaName nextAreaName)
    {
        return ArchipelagoMapMod.GS.ShowNextAreas switch
        {
            NextAreaSetting.Off or NextAreaSetting.Arrows => false,
            NextAreaSetting.Full or _ => null
        };
    }

    public override bool? NextAreaArrowActiveOverride(NextAreaArrow nextAreaArrow)
    {
        return ArchipelagoMapMod.GS.ShowNextAreas switch
        {
            NextAreaSetting.Off => false,
            NextAreaSetting.Arrows or NextAreaSetting.Full or _ => null
        };
    }

    public override Vector4? NextAreaColorOverride(MiscObjectDef miscObjectDef)
    {
        return GetCustomColor(miscObjectDef.ColorSetting);
    }


    private Vector4? GetCustomColor(ColorSetting colorSetting)
    {
        var customColor = apmmColors.GetColor(colorSetting);

        if (!customColor.Equals(Vector4.negativeInfinity)) return customColor.ToOpaque();

        return null;
    }

    public override Vector4? QuickMapTitleColorOverride(QuickMapTitle qmt)
    {
        var customColor = apmmColors.GetColorFromMapZone(Finder.GetCurrentMapZone());

        if (!customColor.Equals(Vector4.negativeInfinity)) return customColor.ToOpaque();

        return null;
    }
}

internal class FullMapMode : ItemRandoMode
{
    public override string Mod => ArchipelagoMapMod.MOD;
    public override string ModeName => apmmMode.Full_Map.ToString().ToCleanName();

    public override bool? NextAreaNameActiveOverride(NextAreaName nextAreaName)
    {
        return ArchipelagoMapMod.GS.ShowNextAreas switch
        {
            NextAreaSetting.Off or NextAreaSetting.Arrows => false,
            NextAreaSetting.Full or _ => true
        };
    }

    public override bool? NextAreaArrowActiveOverride(NextAreaArrow nextAreaArrow)
    {
        return ArchipelagoMapMod.GS.ShowNextAreas switch
        {
            NextAreaSetting.Off => false,
            NextAreaSetting.Arrows or NextAreaSetting.Full or _ => true
        };
    }
}

internal class AllPinsMode : ItemRandoMode
{
    public override string Mod => ArchipelagoMapMod.MOD;
    public override string ModeName => apmmMode.All_Pins.ToString().ToCleanName();
    public override bool FullMap => false;
}

internal class PinsOverAreaMode : ItemRandoMode
{
    public override string Mod => ArchipelagoMapMod.MOD;
    public override string ModeName => apmmMode.Pins_Over_Area.ToString().ToCleanName();
    public override bool FullMap => false;
}

internal class PinsOverRoomMode : ItemRandoMode
{
    public override string Mod => ArchipelagoMapMod.MOD;
    public override string ModeName => apmmMode.Pins_Over_Room.ToString().ToCleanName();
    public override bool FullMap => false;
}