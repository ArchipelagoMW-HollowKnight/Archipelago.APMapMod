using ArchipelagoMapMod.Settings;
using ArchipelagoMapMod.Transition;
using MapChanger;
using MapChanger.MonoBehaviours;
using UnityEngine;

namespace ArchipelagoMapMod.Modes;

internal abstract class TransitionRandoMode : apmmMapMode
{
    public override bool DisableAreaNames => true;

    public override bool InitializeToThis()
    {
        if (!TransitionData.IsTransitionRando()) return false;

        return ModeName == ArchipelagoMapMod.GS.DefaultTransitionRandoMode.ToString().ToCleanName();
    }

    public override bool? RoomActiveOverride(RoomSprite roomSprite)
    {
        return TransitionTracker.GetRoomActive(roomSprite.Rsd.SceneName);
    }

    public override Vector4? RoomColorOverride(RoomSprite roomSprite)
    {
        return roomSprite.Selected
            ? APmmColors.GetColor(APmmColorSetting.Room_Selected)
            : TransitionTracker.GetRoomColor(roomSprite.Rsd.SceneName);
    }

    public override Vector4? QuickMapTitleColorOverride(QuickMapTitle qmt)
    {
        return APmmColors.GetColor(ColorSetting.UI_Neutral);
    }

    public override bool? NextAreaNameActiveOverride(NextAreaName nextAreaName)
    {
        return false;
    }

    public override bool? NextAreaArrowActiveOverride(NextAreaArrow nextAreaArrow)
    {
        return false;
    }
}

internal class TransitionNormalMode : TransitionRandoMode
{
    public override string Mod => ArchipelagoMapMod.MOD;
    public override string ModeName => apmmMode.Transition_Normal.ToString().ToCleanName();
}

internal class TransitionVisitedOnlyMode : TransitionRandoMode
{
    public override string Mod => ArchipelagoMapMod.MOD;
    public override string ModeName => apmmMode.Transition_Visited_Only.ToString().ToCleanName();
}

internal class TransitionAllRoomsMode : TransitionRandoMode
{
    public override string Mod => ArchipelagoMapMod.MOD;
    public override string ModeName => apmmMode.Transition_All_Rooms.ToString().ToCleanName();
}