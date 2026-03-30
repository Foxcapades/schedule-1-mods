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
using System.IO;
#endif

[assembly: MelonGame("TVGS", "Schedule I")]
[assembly: MelonInfo(typeof(OscarsInventoryTweaks.Mod), OscarsInventoryTweaks.Mod.MOD_ID, "1.0.1", "Foxcapades")]

#nullable enable
namespace OscarsInventoryTweaks {

  public class Mod: FxMod<Mod> {
    public const string MOD_ID = "Oscar's Inventory Tweaks";

    protected override string configPath => "OscarsInventoryTweaks.cfg";

    internal static readonly string[] TargetItemIDs = new string[] {
      Item.Fertilizer,
      Item.PGR,
      Item.SpeedGrow,

      Item.Locker,

      Item.SmallStorageCloset,
      Item.MediumStorageCloset,
      Item.LargeStorageCloset,
      Item.HugeStorageCloset,

      Item.SmallStorageRack,
      Item.MediumStorageRack,
      Item.LargeStorageRack,
    };

    private static readonly ShopListing[] targetItemListings = new ShopListing[TargetItemIDs.Length];

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
      Logger.Debug("initializing");

      var dan = NPC.Get<Dan>(NPC.Dan)!.ShopInterface;

      for (var i = 0; i < TargetItemIDs.Length; i++) {
        targetItemListings[i] = dan.GetListing(TargetItemIDs[i]);

        Logger.Debug("grabbed {0} listing from dan", targetItemListings[i]?.Item?.ID);
      }
    }

    private static void moveItem<T>(List<T> items, int from, int to) {
      var item = items[from];
      items.RemoveAt(from);
      items.Insert(to, item);
    }

    private static Dictionary<string, ShopListing?> buildListings() {
      var listings = new Dictionary<string, ShopListing?>(TargetItemIDs.Length);

      for (var i = 0; i < TargetItemIDs.Length; i++) {
        listings[TargetItemIDs[i]] = preferences!.IsEnabled(TargetItemIDs[i])
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
          Logger.Debug("removing {0} from Oscar's inventory", itemID);
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

      for (var i = TargetItemIDs.Length - 1; i >= 0; i--) {
        var item = TargetItemIDs[i];

        if (!newListings.TryGetValue(item, out var value))
          continue;

        if (value == null)
          continue;

        oscar.Listings.Add(Utils.copyListing(targetItemListings[i], oscar));

        #if IL2CPP
        oscar.CreateListingUI(value);
        #elif MONO
        inputs[0] = value;
        createListingUI.Invoke(oscar, inputs);
        #endif

        // Find the element after which we want to insert the new UI element.
        var afterID = value.AppearsAfter();
        var j = 0;
        for (; j < uiListings.Count; j++)
          if (uiListings[j].Listing.Item.ID == afterID)
            break;

        // If the target was found, move the index of the element 1 ahead.
        if (j < uiListings.Count)
          j++;

        // If the target element was found and was not the last item in the list
        // move the new UI element to the correct position.
        if (j < uiListings.Count)
          moveItem(uiListings, uiListings.Count-1, j);
      }
    }

    private static void updateShopListings() {
      var oscar = NPC.Get<Oscar>(NPC.Oscar)?.ShopInterface;

      if (oscar == null) {
        Logger.Debug("oscar was null");
        return;
      }

      var uiListings = oscar.ListingUIItems();
      if (uiListings == null) {
        Logger.Debug("uiListings was null");
        return;
      }

      var uiPanel = oscar.ListingPanel();
      if (uiPanel == null) {
        Logger.Debug("uiPanel was null");
        return;
      }

      var newListings = buildListings();

      pruneDisabledListings(oscar, newListings, uiListings, uiPanel);
      injectNewListings(oscar, newListings, uiListings);
    }
  }
}
