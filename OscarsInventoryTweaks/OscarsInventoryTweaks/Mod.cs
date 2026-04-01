using MelonLoader;
using Fxcpds;

#if IL2CPP
using Il2CppScheduleOne;
using Il2CppScheduleOne.NPCs.CharacterClasses;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.UI.Shop;
using Il2CppSystem.Collections.Generic;
#elif MONO
using ScheduleOne;
using ScheduleOne.NPCs.CharacterClasses;
using ScheduleOne.PlayerScripts;
using ScheduleOne.UI.Shop;
using System.Collections.Generic;
using System.Reflection;
#endif

[assembly: MelonGame("TVGS", "Schedule I")]
[assembly: MelonInfo(typeof(OscarsInventoryTweaks.Mod), OscarsInventoryTweaks.Mod.MOD_ID, "1.2.0", "Foxcapades")]

#nullable enable
namespace OscarsInventoryTweaks {
  public class Mod: FxMod {
    public const string MOD_ID = "Oscar's Inventory Tweaks";

    protected override string configPath => "OscarsInventoryTweaks.cfg";

    private static readonly ShopListing[] targetItemListings = new ShopListing[Item.itemCount];

    private static Preferences? preferences;

    public override void OnInitializeMelon() {
      preferences = new Preferences(ConfigPath!);
      base.OnInitializeMelon();
    }

    protected override void onModPreferencesSaved() {
      if (!InMainScene)
        return;

      updateShopListings();
    }

    protected override void onLocalPlayerLoaded(Player _) {
      init();
      updateShopListings();
    }

    private static void init() {
      Instance.LoggerInstance.Debug("initializing");

      var dan = NPC.Get<Dan>(NPC.Dan)!.ShopInterface;

      for (var i = 0; i < Item.itemCount; i++) {
        targetItemListings[i] = dan.GetListing(Item.allItems[i].id);

        Instance.LoggerInstance.Debug("grabbed {0} listing from dan", targetItemListings[i].Item.ID);
      }
    }

    private static void moveItem<T>(List<T> items, int from, int to) {
      var item = items[from];
      items.RemoveAt(from);
      items.Insert(to, item);
    }

    private static Dictionary<string, ShopListing?> buildListings() {
      var listings = new Dictionary<string, ShopListing?>(Item.itemCount);

      var itemPrefs = preferences!.getItems();

      for (var i = 0; i < itemPrefs.Length; i++) {
        var (item, enabled) = itemPrefs[i];

        listings[item.id] = enabled
          ? targetItemListings[i]
          : null;
      }

      return listings;
    }

    private static void pruneDisabledListings(
      ShopInterface oscar,
      Dictionary<string, ShopListing?> newListings,
      List<ListingUI> uiListings,
      UIPanel listingPanel
    ) {
      // Remove any entries that were previously enabled, but are now disabled.
      foreach (var (itemID, record) in newListings) {
        if (record != null)
          continue;

        var result = oscar.GetListing(itemID);

        if (result != null) {
          Instance.LoggerInstance.Debug("removing {0} from Oscar's inventory", itemID);
          oscar.Listings.Remove(result);

          ListingUI? uiListing = null;

          for (var i = 0; i < uiListings.Count; i++) {
            if (uiListings[i].Listing.Item.ID == itemID) {
              uiListing = uiListings[i];
              uiListings.RemoveAt(i);
              break;
            }
          }

          if (uiListing == null)
            continue;

          listingPanel.RemoveSelectable(uiListing.GetComponent<UISelectable>());

          UnityEngine.Object.Destroy(uiListing.gameObject);
        }
      }
    }

    private static void injectNewListings(
      ShopInterface oscar,
      Dictionary<string, ShopListing?> newListings,
      List<ListingUI> uiListings
    ) {
      #if MONO
      var createListingUI = typeof(ShopInterface)
        .GetMethod("CreateListingUI", BindingFlags.NonPublic | BindingFlags.Instance, null, new [] { typeof(ShopListing) }, null)!;
      var inputs = new object[1];
      #endif

      // Remove listings that oscar already has from the newListing dict
      foreach (var listing in oscar.Listings) {
        newListings.Remove(listing.Item.ID);
      }

      for (var i = Item.allItems.Length - 1; i >= 0; i--) {
        var item = Item.allItems[i];

        // If we don't have a listing for the item, don't try and insert it.
        if (!newListings.TryGetValue(item.id, out var value))
          continue;

        // If the listing was null, the item is disabled via prefs.
        if (value == null)
          continue;

        oscar.Listings.Add(Utils.copyListing(targetItemListings[i], oscar));

        #if IL2CPP
        oscar.CreateListingUI(value);
        #elif MONO
        inputs[0] = value;
        createListingUI.Invoke(oscar, inputs);
        #endif

        var insertAt = findInsertPosition(uiListings, item.category.appearsAfter);

        if (insertAt > -1)
          moveItem(uiListings, uiListings.Count-1, insertAt);
      }
    }

    private static int findInsertPosition(List<ListingUI> list, string target) {
      for (var i = 0; i < list.Count; i++) {
        if (list[i].Listing.Item.ID == target)
          return i + 1;
      }

      return -1;
    }

    private static void updateShopListings() {
      var oscar = NPC.Get<Oscar>(NPC.Oscar)?.ShopInterface;

      if (oscar == null) {
        Instance.LoggerInstance.Debug("oscar was null");
        return;
      }

      var uiListings = oscar.ListingUIItems();
      if (uiListings == null) {
        Instance.LoggerInstance.Debug("uiListings was null");
        return;
      }

      var uiPanel = oscar.ListingPanel();
      if (uiPanel == null) {
        Instance.LoggerInstance.Debug("uiPanel was null");
        return;
      }

      var newListings = buildListings();

      pruneDisabledListings(oscar, newListings, uiListings, uiPanel);
      injectNewListings(oscar, newListings, uiListings);
    }
  }
}
