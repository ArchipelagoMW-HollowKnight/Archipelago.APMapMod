using Archipelago.HollowKnight.IC;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using ArchipelagoMapMod.Pins;
using ItemChanger;
using ItemChanger.Internal;
using MagicUI.Core;
using MagicUI.Elements;
using MapChanger;
using UnityEngine.UI;
using HT = Archipelago.HollowKnight.HintTracker;

namespace ArchipelagoMapMod.UI;

public static class HintDisplay
{
    private static LayoutRoot _layout;
    private static readonly List<TextFormatter<Hint>> Formatters = new();

    private static ArchipelagoSession Session => Archipelago.HollowKnight.Archipelago.Instance.session;

    private static bool _visible = true;
    private const int MaxHints = 20;
    private static int _hintsShown = ArchipelagoMapMod.GS.gameplayHints;

    public static void Make()
    {
        Destroy();

        On.InvAnimateUpAndDown.AnimateUp += OpenInv;
        On.InvAnimateUpAndDown.AnimateDown += CloseInv;
        On.UIManager.UIGoToPauseMenu += OpenPause;
        On.UIManager.UIClosePauseMenu += ClosePause;
        Archipelago.HollowKnight.HintTracker.OnArchipelagoHintUpdate += SortHints;

        if (_layout == null)
        {
            ArchipelagoMapMod.Instance.Log("Creating hint display");
            _layout = new(true, "Hint Display");
            //layout.RenderDebugLayoutBounds = true;
            _layout.VisibilityCondition = () => !(States.WorldMapOpen || States.QuickMapOpen) && _visible;
            _layout.Interactive = false;

            StackLayout hintLayout = new(_layout, "Hints")
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Orientation = Orientation.Vertical,
                Padding = new(0, 0, 10f, 10f),
                Spacing = 5f
            };

            for (var i = 0; i < MaxHints; i++)
            {
                TextObject hintText = new(_layout)
                {
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center,
                    Font = MagicUI.Core.UI.Perpetua,
                    FontSize =  ArchipelagoMapMod.GS.hintFontSize,
                };
                TextFormatter<Hint> f = new(_layout, null, FormatHint)
                {
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center,
                    Text = hintText,
                    Visibility = Visibility.Collapsed
                };
                hintText.GameObject.GetComponent<Text>().supportRichText = true;
                Formatters.Add(f);
            }

            foreach (var formatter in Formatters)
            {
                hintLayout.Children.Add(formatter);
            }
        }
        
        SortHints();
        UpdateDisplay();
    }

    public static void Destroy()
    {
        On.InvAnimateUpAndDown.AnimateUp -= OpenInv;
        On.InvAnimateUpAndDown.AnimateDown -= CloseInv;
        On.UIManager.UIGoToPauseMenu -= OpenPause;
        On.UIManager.UIClosePauseMenu -= ClosePause;
        
        _layout?.Destroy();
        _layout = null;
    }

    private static void ClosePause(On.UIManager.orig_UIClosePauseMenu orig, UIManager self)
    {
        orig(self);
        _visible = true;
        UpdateDisplay(ArchipelagoMapMod.GS.gameplayHints);
    }

    private static void OpenPause(On.UIManager.orig_UIGoToPauseMenu orig, UIManager self)
    {
        orig(self);
        _visible = true;
        UpdateDisplay(ArchipelagoMapMod.GS.pauseMenuHints);
    }

    private static void OpenInv(On.InvAnimateUpAndDown.orig_AnimateUp orig, InvAnimateUpAndDown self)
    {
        orig(self);
        _visible = false;
    }


    private static void CloseInv(On.InvAnimateUpAndDown.orig_AnimateDown orig, InvAnimateUpAndDown self)
    {
        orig(self);
        _visible = true;
    }

    private static void SortHints()
    {
        HT.Hints.Sort((hint1, hint2) =>
        {
            // if same location return 0
            if (hint1.LocationId == hint2.LocationId)
                return 0;

            var hint1Color = GetColor(hint1.LocationId);
            var hint2Color = GetColor(hint2.LocationId);
            
            // if same color sort by location id
            if( hint1Color == hint2Color)
                return hint1.LocationId >= hint2.LocationId ? -1 : 1;
            
            // green first
            if (hint1Color == ColorResult.Green &&
                hint2Color != ColorResult.Green)
                return -1;
            if (hint1Color != ColorResult.Green &&
                hint2Color == ColorResult.Green)
                return 1;

            // yellow second
            if (hint1Color == ColorResult.Yellow &&
                hint2Color != ColorResult.Yellow)
                return -1;
            if (hint1Color != ColorResult.Yellow &&
                hint2Color == ColorResult.Yellow)
                return 1;

            // Red third
            if (hint1Color == ColorResult.Red &&
                hint2Color != ColorResult.Red)
                return -1;
            if (hint1Color != ColorResult.Red &&
                hint2Color == ColorResult.Red)
                return 1;

            // how did we get here?
            return 0;
        });
        
        UpdateDisplay();
    }

    public static void UpdateDisplay(int hintsToShow)
    {
        _hintsShown = hintsToShow;
        UpdateDisplay();
    }

    public static void UpdateDisplay()
    {
        if (!(Archipelago.HollowKnight.Archipelago.Instance?.ArchipelagoEnabled).GetValueOrDefault(false))
            return;
        
        int shown = 0;
        foreach (var hint in HT.Hints)
        {
            if (hint.FindingPlayer != Session.ConnectionInfo.Slot)
                continue;
            if (hint.Found)
                continue;
            if(!hint.ItemFlags.HasFlag(ItemFlags.Advancement))
                continue;

            var locationString = Session.Locations.GetLocationNameFromId(hint.LocationId);
            if (GetColor(hint.LocationId) == ColorResult.Obtained)
                return;

            if (ArchipelagoMapMod.LS.TrackerData.clearedLocations.Contains(locationString))
                continue;

            int x = Formatters.Count - 1 - shown;
            Formatters[x].Data = hint;
            Formatters[x].Visibility =
                shown >= _hintsShown ? Visibility.Collapsed : Visibility.Visible;
            Formatters[x].Text.FontSize = ArchipelagoMapMod.GS.hintFontSize;
            shown++;
            if (shown >= MaxHints)
                break;
        }
    }

    private static bool IsShop(string location)
    {
        if (string.IsNullOrEmpty(location))
            return false;

        var names = new[]
        {
            LocationNames.Sly_Key, LocationNames.Sly, LocationNames.Iselda, LocationNames.Salubra,
            LocationNames.Leg_Eater, LocationNames.Egg_Shop, LocationNames.Seer, LocationNames.Grubfather
        };

        foreach (var name in names)
        {
            if (location.StartsWith(name))
            {
                return true;
            }
        }

        return false;
    }

    private static string StripShopSuffix(string location)
    {
        if (string.IsNullOrEmpty(location))
        {
            return null;
        }

        var names = new[]
        {
            LocationNames.Sly_Key, LocationNames.Sly, LocationNames.Iselda, LocationNames.Salubra,
            LocationNames.Leg_Eater, LocationNames.Egg_Shop, LocationNames.Seer, LocationNames.Grubfather
        };

        foreach (var name in names)
        {
            if (location.StartsWith(name))
            {
                return location.Substring(0, name.Length);
            }
        }

        return location;
    }

    private enum ColorResult
    {
        Obtained,
        Green,
        Yellow,
        Red
    }

    private static ColorResult GetColor(long locationID)
    {
        var locationName = StripShopSuffix(Session.Locations.GetLocationNameFromId(locationID));
        if (!Ref.Settings.Placements.ContainsKey(locationName)) return ColorResult.Red;

        var geo = true;
        var other = true;
        RandomizedAPmmPin pin = (RandomizedAPmmPin) APmmPinManager.Pins[locationName];
        if (pin.placementState is RandoPlacementState.UncheckedUnreachable or RandoPlacementState.PreviewedUnreachable)
            return ColorResult.Red;


        foreach (var item in Ref.Settings.Placements[locationName].Items)
        {
            // if for some reason this is not a AP taggable item just continue past it
            if (!item.HasTag<ArchipelagoItemTag>())
                continue;
            if (item.GetTag<ArchipelagoItemTag>().Location != locationID)
                continue;

            if (item.WasEverObtained())
                return ColorResult.Obtained;
            
            // this is our item and it has no cost yay! return green
            if (!item.HasTag<CostTag>())
                return ColorResult.Green;

            if (item.GetTag<CostTag>().Cost is MultiCost costs)
            {
                //multicost cycle though all costs and check if they are obtainable
                foreach (var cost in costs)
                {
                    if (cost is GeoCost geoCost)
                    {
                        if (!geoCost.CanPay())
                            geo = false;
                    }
                    else
                    {
                        if (cost.CanPay()) continue;
                        other = false;
                        break;
                    }
                }
            }
            else
            {
                //single cost check of obtainable.
                if (item.GetTag<CostTag>().Cost is GeoCost geoCost)
                {
                    if (!geoCost.CanPay())
                        geo = false;
                }
                else if (!item.GetTag<CostTag>().Cost.CanPay())
                {
                    other = false;
                }
            }

            break;
        }
        if (other)
        {
            return geo ? ColorResult.Green : ColorResult.Yellow;
        }
        return ColorResult.Red;
    }

    private static string FormatHint(Hint hint)
        {
            if (hint == null) return "";
            var color = GetColor(hint.LocationId) switch
            {
                ColorResult.Green => "green",
                ColorResult.Yellow => "yellow",
                ColorResult.Red => "red",
                _ => "grey"
            };
            
            return
                $"<b>{Session.Players.GetPlayerAlias(hint.ReceivingPlayer)}</b>'s <b>[{Session.Items.GetItemName(hint.ItemId)}]</b> from <b><color={color}>[{Session.Locations.GetLocationNameFromId(hint.LocationId).Replace("_", " ")}]</color></b>";
        }
    }