using MelonLoader;
using Fxcpds;
#if IL2CPP
using Il2CppInterop.Runtime;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.NPCs.CharacterClasses;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.UI.Shop;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
#elif MONO
using ScheduleOne.NPCs;
using ScheduleOne.NPCs.CharacterClasses;
using ScheduleOne.PlayerScripts;
using ScheduleOne.UI.Shop;
using System.Collections.Generic;
using System.Reflection;
#endif

[assembly: MelonInfo(typeof(OscarsInventoryTweaks.Mod), OscarsInventoryTweaks.Mod.MOD_NAME, "1.0.0", "Foxcapades")]
[assembly: MelonGame("TVGS", "Schedule I")]

#nullable enable
namespace OscarsInventoryTweaks {

  public class Mod: MelonMod {
    public const string MOD_ID   = "OscarsInventoryTweaks";
    public const string MOD_NAME = "Oscar's Inventory Tweaks";

    private static MelonPreferences_Entry<bool>? sellAdditives;

    private static MelonLogger.Instance Logger => Melon<Mod>.Logger;

    public override void OnInitializeMelon() {
      var category = MelonPreferences.CreateCategory(MOD_ID, MOD_NAME);

      sellAdditives = category.CreateEntry(
        identifier:    "sellAdditives",
        default_value: true,
        display_name:  "Sell Additives",
        description:   "Lets Oscar sell plant growth additives."
      );

      #if IL2CPP
      Player.onPlayerSpawned += DelegateSupport.ConvertDelegate<Action<Player>>(onPlayerSpawn);
      #elif MONO
      Player.onPlayerSpawned += onPlayerSpawn;
      #endif
    }

    private static void onPlayerSpawn(Player player) {
      if (player.IsLocalPlayer) {
        updateShopListings();
      }
    }

    private readonly struct Definitions {
      public readonly ShopListing locker;
      public readonly ShopListing fertilizer;
      public readonly ShopListing pgr;
      public readonly ShopListing speedGrow;

      public Definitions(
        ShopListing locker,
        ShopListing fertilizer,
        ShopListing pgr,
        ShopListing speedGrow
      ) {
        this.locker = locker;
        this.fertilizer = fertilizer;
        this.pgr = pgr;
        this.speedGrow = speedGrow;
      }
    }

    #if IL2CPP
    private static void createListingUI(ShopInterface shop, ShopListing[] listings) {
      foreach (var listing in listings) {
        shop.CreateListingUI(listing);
      }
    }
    #elif MONO
    private static void createListingUI(ShopInterface shop, ShopListing[] listings) {
      var method = typeof(ShopInterface)
        .GetMethod("CreateListingUI", BindingFlags.NonPublic, null, new [] { typeof(ShopListing) }, null)!;

      foreach (var listing in listings) {
        method.Invoke(shop, new object[] { listing });
      }
    }
    #endif

    private static Definitions getDefsFromDan() {
      var dan = Interop.cast<Dan>(NPCManager.GetNPC(Fxcpds.NPC.DAN))!;

      return new Definitions(
        dan.ShopInterface.GetListing(Item.LOCKER),
        dan.ShopInterface.GetListing(Item.FERTILIZER),
        dan.ShopInterface.GetListing(Item.PGR),
        dan.ShopInterface.GetListing(Item.SPEED_GROW)
      );
    }

    private static ShopListing copyListing(ShopListing old, ShopInterface shop) {
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

    private static List<ListingUI> getListingUIList(ShopInterface shop) {
      #if IL2CPP
      return shop.listingUI;
      #elif MONO
      return (List<ListingUI>) typeof(ShopInterface)
        .GetProperty("listingUI", BindingFlags.NonPublic)
        !.GetValue(shop);
      #endif
    }

    private static void moveItem<T>(List<T> items, int from, int to) {
      var item = items[from];
      items.RemoveAt(from);
      items.Insert(to, item);
    }

    private static void updateShopListings() {
      var definitions = getDefsFromDan();
      var oscar = Interop.cast<Oscar>(NPCManager.GetNPC(Fxcpds.NPC.OSCAR));

      if (oscar == null) {
        Logger.Error("couldn't find Oscar :C");
        return;
      }

      var listings = oscar.ShopInterface.Listings;

      var newListings = new ShopListing[4] {
        copyListing(definitions.fertilizer, oscar.ShopInterface),
        copyListing(definitions.pgr, oscar.ShopInterface),
        copyListing(definitions.speedGrow, oscar.ShopInterface),
        copyListing(definitions.locker, oscar.ShopInterface),
      };

      foreach (var listing in newListings)
        listings.Add(listing);

      var listingUI = getListingUIList(oscar.ShopInterface);
      var newPositions = new int[4];

      for (var i = 0; i < listingUI.Count; i++) {
        var id = listingUI[i].Listing.Item.ID;

        if (id == Item.BED) {
          newPositions[3] = i + 1;
        } else if (id == Item.TIER_3_SOIL) {
          for (var j = 0; j < 3; j++) {
            newPositions[j] = i + 1;
          }
        }
      }

      createListingUI(oscar.ShopInterface, newListings);

      for (var i = newPositions.Length - 1; i >= 0; i--)
        moveItem(listingUI, listingUI.Count-1, newPositions[i]);
    }
  }
}
