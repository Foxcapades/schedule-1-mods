#if IL2CPP
using Il2CppScheduleOne;
using Il2CppScheduleOne.UI.Shop;
using Il2CppSystem.Collections.Generic;
#elif MONO
using Fxcpds;
using ScheduleOne;
using ScheduleOne.UI.Shop;
using System.Collections.Generic;
using System.Reflection;
#endif

#nullable enable
namespace OscarsInventoryTweaks {
  internal static class Utils {

    // IMPORTANT!!!!!
    // I did a stupid, and tied the order of this array to the order of the all
    // items array.  THESE MUST REMAIN IN THE SAME ORDER UNTIL I GET AROUND TO
    // FIXING THIS.
    public static (Item, bool)[] getItems(this Preferences prefs) =>
      new [] {
        (Item.Fertilizer, prefs.SellFertilizer),
        (Item.PGR, prefs.SellPgr),
        (Item.SpeedGrow, prefs.SellSpeedGrow),

        (Item.Locker, prefs.SellLocker),

        (Item.BigSprinkler, prefs.SellBigSprinkler),
        (Item.AirConditioner, prefs.SellAirCon),

        (Item.SmallStorageCloset, prefs.SellClosets),
        (Item.MediumStorageCloset, prefs.SellClosets),
        (Item.LargeStorageCloset, prefs.SellClosets),
        (Item.HugeStorageCloset, prefs.SellClosets),

        (Item.SmallStorageRack, prefs.SellShelves),
        (Item.MediumStorageRack, prefs.SellShelves),
        (Item.LargeStorageRack, prefs.SellShelves),
      };

    public static List<ListingUI>? ListingUIItems(this ShopInterface shop) {
      #if IL2CPP
      return shop.listingUI;
      #elif MONO
      var field = typeof(ShopInterface)
        .GetField("listingUI", BindingFlags.NonPublic | BindingFlags.Instance);

      FxMod.Instance.LoggerInstance.Debug("reflectively got field {0}", field);

      return (List<ListingUI>) field!.GetValue(shop);
      #endif
    }

    public static UIPanel? ListingPanel(this ShopInterface shop) {
      #if IL2CPP
      return shop.listingPanel;
      #elif MONO
      var field = typeof(ShopInterface)
        .GetField("listingPanel", BindingFlags.NonPublic | BindingFlags.Instance);

      FxMod.Instance.LoggerInstance.Debug("reflectively got field {0}", field);

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