using Archipelago.HollowKnight.IC;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;
using ArchipelagoMapMod.Pins;
using ItemChanger;
using ItemChanger.Internal;
using ItemChanger.Placements;
using MagicUI.Core;
using MagicUI.Elements;
using MapChanger;
using UnityEngine.UI;
using HT = Archipelago.HollowKnight.HintTracker;

namespace ArchipelagoMapMod.UI;

public static class HintDisplay
{
    private static LayoutRoot _layout;
    private static List<TextFormatter<Hint>> _formatters;

    private static ArchipelagoSession Session => Archipelago.HollowKnight.Archipelago.Instance.session;

    private static bool _visible = true;
    private const int MaxHints = 20;
    private static int _hintsShown = ArchipelagoMapMod.GS.GameplayHints;

    public static void Make()
    {
        Destroy();

        On.InvAnimateUpAndDown.AnimateUp += OpenInv;
        On.InvAnimateUpAndDown.AnimateDown += CloseInv;
        On.UIManager.UIGoToPauseMenu += OpenPause;
        On.UIManager.UIClosePauseMenu += ClosePause;
        HT.OnArchipelagoHintUpdate += SortHints;

        _visible = true;
        _hintsShown = ArchipelagoMapMod.GS.GameplayHints;

        if (_layout == null)
        {
            ArchipelagoMapMod.Instance.Log("Creating hint display");
            _layout = new LayoutRoot(true, "Hint Display");
            _formatters = [];
            //_layout.RenderDebugLayoutBounds = true;
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

            for (int i = 0; i < MaxHints; i++)
            {
                TextObject hintText = new(_layout)
                {
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center,
                    Font = MagicUI.Core.UI.Perpetua,
                    FontSize = ArchipelagoMapMod.GS.HintFontSize,
                };
                TextFormatter<Hint> f = new(_layout, null, FormatHint)
                {
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center,
                    Text = hintText,
                    Visibility = Visibility.Collapsed
                };
                hintText.GameObject.GetComponent<Text>().supportRichText = true;
                _formatters.Add(f);
            }

            foreach (TextFormatter<Hint> formatter in _formatters)
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
        _formatters = null;
    }

    private static void ClosePause(On.UIManager.orig_UIClosePauseMenu orig, UIManager self)
    {
        orig(self);
        _visible = true;
        UpdateDisplay(ArchipelagoMapMod.GS.GameplayHints);
    }

    private static void OpenPause(On.UIManager.orig_UIGoToPauseMenu orig, UIManager self)
    {
        orig(self);
        _visible = true;
        UpdateDisplay(ArchipelagoMapMod.GS.PauseMenuHints);
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
            {
                return 0;
            }

            ColorResult hint1Color = GetColor(hint1.LocationId);
            ColorResult hint2Color = GetColor(hint2.LocationId);

            // if same color sort by location id
            if (hint1Color == hint2Color)
            {
                return hint1.LocationId >= hint2.LocationId ? -1 : 1;
            }

            // green first
            if (hint1Color == ColorResult.Green &&
                hint2Color != ColorResult.Green)
            {
                return -1;
            }

            if (hint1Color != ColorResult.Green &&
                hint2Color == ColorResult.Green)
            {
                return 1;
            }

            // yellow second
            if (hint1Color == ColorResult.Yellow &&
                hint2Color != ColorResult.Yellow)
            {
                return -1;
            }

            if (hint1Color != ColorResult.Yellow &&
                hint2Color == ColorResult.Yellow)
            {
                return 1;
            }

            // Red third
            if (hint1Color == ColorResult.Red &&
                hint2Color != ColorResult.Red)
            {
                return -1;
            }

            if (hint1Color != ColorResult.Red &&
                hint2Color == ColorResult.Red)
            {
                return 1;
            }

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
        {
            return;
        }

        if (_layout == null || _formatters == null)
        {
            return;
        }

        int shown = 0;
        foreach (Hint hint in HT.Hints)
        {
            if (hint.FindingPlayer != Session.ConnectionInfo.Slot)
            {
                continue;
            }

            if (hint.Found)
            {
                continue;
            }

            if (!hint.ItemFlags.HasFlag(ItemFlags.Advancement))
            {
                continue;
            }

            string locationString = Session.Locations.GetLocationNameFromId(hint.LocationId);
            if (GetColor(hint.LocationId) == ColorResult.Obtained)
            {
                return;
            }

            if (ArchipelagoMapMod.LS.TrackerData.clearedLocations.Contains(locationString))
            {
                continue;
            }

            int x = _formatters.Count - 1 - shown;
            _formatters[x].Data = hint;
            _formatters[x].Visibility =
                shown >= _hintsShown ? Visibility.Collapsed : Visibility.Visible;
            _formatters[x].Text.FontSize = ArchipelagoMapMod.GS.HintFontSize;
            shown++;
            if (shown >= MaxHints)
            {
                break;
            }
        }
    }

    private static string StripShopSuffix(string location)
    {
        if (string.IsNullOrEmpty(location))
        {
            return null;
        }

        string[] names =
        {
            LocationNames.Sly_Key, LocationNames.Sly, LocationNames.Iselda, LocationNames.Salubra,
            LocationNames.Leg_Eater, LocationNames.Egg_Shop, LocationNames.Seer, LocationNames.Grubfather
        };

        foreach (string name in names)
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
        string locationName = StripShopSuffix(Session.Locations.GetLocationNameFromId(locationID));
        if (locationName == null || !Ref.Settings.Placements.ContainsKey(locationName))
        {
            return ColorResult.Red;
        }

        bool geo = true;

        // hints can come though for vanilla items, if that is the case then mark them as obtained so it does not display on the hint tracker
        if (!APmmPinManager.Pins.TryGetValue(locationName, out APmmPin generalPin) || generalPin is not RandomizedAPmmPin pin)
        {
            return ColorResult.Obtained;
        }

        if (pin.placementState is RandoPlacementState.UncheckedUnreachable or RandoPlacementState.PreviewedUnreachable)
        {
            return ColorResult.Red;
        }

        AbstractPlacement pmt = Ref.Settings.Placements[locationName];
        foreach (AbstractItem item in pmt.Items.Where(item => item.GetTag<ArchipelagoItemTag>()?.Location == locationID))
        {
            if (item.WasEverObtained())
            {
                return ColorResult.Obtained;
            }

            List<Cost> costs = GetCosts(item, pmt);

            if (costs.Count == 0)
            {
                return ColorResult.Green;
            }

            foreach (Cost cost in costs)
            {
                if (cost is GeoCost geoCost)
                {
                    if (!geoCost.CanPay())
                    {
                        geo = false;
                    }
                }
                else
                {
                    if (cost.CanPay())
                    {
                        continue;
                    }

                    return ColorResult.Red;
                }
            }
            break;
        }
        return geo ? ColorResult.Green : ColorResult.Yellow;
    }

    private static List<Cost> GetCosts(AbstractItem item, AbstractPlacement placement)
    {
        List<Cost> costs = [];
        if (placement is ISingleCostPlacement iscp)
        {
            Cost cost = iscp.Cost;
            if (cost is MultiCost multiCost)
            {
                costs.AddRange(multiCost);
            }
            else if (cost != null)
            {
                costs.Add(cost);
            }
        }
        if (item.GetTag(out CostTag costTag))
        {
            Cost cost = costTag.Cost;
            if (cost is MultiCost multiCost)
            {
                costs.AddRange(multiCost);
            }
            else
            {
                costs.Add(cost);
            }
        }

        return costs;
    }

    private static string FormatHint(Hint hint)
    {
        if (hint == null)
        {
            return "";
        }

        string color = GetColor(hint.LocationId) switch
        {
            ColorResult.Green => "green",
            ColorResult.Yellow => "yellow",
            ColorResult.Red => "red",
            _ => "grey"
        };

        PlayerInfo receivingPlayer = Session.Players.GetPlayerInfo(hint.ReceivingPlayer);

        return
            $"<b>{receivingPlayer.Alias}</b>'s <b>[{Session.Items.GetItemName(hint.ItemId, receivingPlayer.Game)}]</b> from <b><color={color}>[{Session.Locations.GetLocationNameFromId(hint.LocationId).Replace("_", " ")}]</color></b>";
    }
}
