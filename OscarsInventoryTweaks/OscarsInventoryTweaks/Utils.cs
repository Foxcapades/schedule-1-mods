using Fxcpds;
#if IL2CPP
using Il2CppScheduleOne;
using Il2CppScheduleOne.UI.Shop;
using Il2CppSystem.Collections.Generic;
#elif MONO
using ScheduleOne;
using ScheduleOne.UI.Shop;
using System.Collections.Generic;
using System.Reflection;
#endif

#nullable enable
namespace OscarsInventoryTweaks {
  internal static class Utils {
    public static bool IsEnabled(this Preferences prefs, string itemID) {
      switch (itemID) {
        case Item.Fertilizer:
          return prefs.SellFertilizer;
        case Item.PGR:
          return prefs.SellPgr;
        case Item.SpeedGrow:
          return prefs.SellSpeedGrow;

        case Item.Locker:
          return prefs.SellLocker;

        case Item.SmallStorageCloset:
        case Item.MediumStorageCloset:
        case Item.LargeStorageCloset:
        case Item.HugeStorageCloset:
          return prefs.SellClosets;

        case Item.SmallStorageRack:
        case Item.MediumStorageRack:
        case Item.LargeStorageRack:
          return prefs.SellShelves;

        default:
          return false;
      }
    }

    public static string AppearsAfter(this ShopListing item) {
      switch (item.Item.ID) {
        case Item.Fertilizer:
        case Item.PGR:
        case Item.SpeedGrow:
          return Item.ExtraLongLifeSoil;

        default:
          return Item.Bed;
      }
    }

    public static List<ListingUI>? ListingUIItems(this ShopInterface shop) {
      #if IL2CPP
      return shop.listingUI;
      #elif MONO
      var field = typeof(ShopInterface)
        .GetField("listingUI", BindingFlags.NonPublic | BindingFlags.Instance);

      Mod.Logger.Debug("reflectively got field {0}", field);

      return (List<ListingUI>) field!.GetValue(shop);
      #endif
    }

    public static UIPanel? ListingPanel(this ShopInterface shop) {
      #if IL2CPP
      return shop.listingPanel;
      #elif MONO
      var field = typeof(ShopInterface)
        .GetField("listingPanel", BindingFlags.NonPublic | BindingFlags.Instance);

      Mod.Logger.Debug("reflectively got field {0}", field);

      return (UIPanel) field!.GetValue(shop);
      #endif
    }

    public static ShopListing copyListing(ShopListing old, ShopInterface shop) {
      var listing = new ShopListing {
        name = old.name,
        Item = old.Item,
        LimitedStock = old.LimitedStock,
        DefaultStock = old.DefaultStock,
        RestockRate = old.RestockRate,
        TieStockToNumberVariable = old.TieStockToNumberVariable,
        StockVariableName = old.StockVariableName,
        TrackPurchases = old.TrackPurchases,
        PurchasedQuantityVariableName = old.PurchasedQuantityVariableName,
        EnforceMinimumGameCreationVersion = old.EnforceMinimumGameCreationVersion,
        MinimumGameCreationVersion = old.MinimumGameCreationVersion,
        CanBeDelivered = old.CanBeDelivered,
        UseIconTint = old.UseIconTint,
        IconTint = old.IconTint,
        ConditionalVisibility = old.ConditionalVisibility,
        ConditionalVisibilityVariableName = old.ConditionalVisibilityVariableName
      };

      listing.Initialize(shop);
      listing.SetStock(old.CurrentStock);

      return listing;
    }
  }
}