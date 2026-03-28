using MelonLoader;
#if IL2CPP
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Runtime;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.NPCs.CharacterClasses;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.UI.Shop;
using Il2CppSystem;
#elif MONO
using ScheduleOne.Core.Items.Framework;
using ScheduleOne.ItemFramework;
using ScheduleOne.NPCs;
using ScheduleOne.NPCs.CharacterClasses;
using ScheduleOne.PlayerScripts;
#endif

[assembly: MelonInfo(typeof(OscarsInventoryTweaks.Mod), OscarsInventoryTweaks.Mod.MOD_NAME, "1.0.0", "Foxcapades")]
[assembly: MelonGame("TVGS", "Schedule I")]

#nullable enable
namespace OscarsInventoryTweaks {
  public class Mod: MelonMod {
    public const string MOD_ID   = "OscarsInventoryTweaks";
    public const string MOD_NAME = "Oscar's Inventory Tweaks";

    private const string NPC_ID_OSCAR = "oscar_holland";
    private const string NPC_ID_DAN   = "dan_samwell";

    private const string BED_ITEM_ID        = "bed";
    private const string FERTILIZER_ITEM_ID = "fertilizer";
    private const string LOCKER_ITEM_ID     = "locker";
    private const string PGR_ITEM_ID        = "pgr";
    private const string SPEED_GROW_ITEM_ID = "speedgrow";

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

    private static Definitions getDefsFromDan() {
      var npc = NPCManager.GetNPC(NPC_ID_DAN);
      var dan = Il2CppObjectPool.Get<Dan>(npc.Pointer);

      return new(
        dan.ShopInterface.GetListing(LOCKER_ITEM_ID),
        dan.ShopInterface.GetListing(FERTILIZER_ITEM_ID),
        dan.ShopInterface.GetListing(PGR_ITEM_ID),
        dan.ShopInterface.GetListing(SPEED_GROW_ITEM_ID)
      );
    }

    private static Oscar? grabAnOscar() {
      var oscar = NPCManager.GetNPC(NPC_ID_OSCAR);

      #if IL2CPP
      return oscar == null ? null : Il2CppObjectPool.Get<Oscar>(oscar.Pointer);
      #elif MONO
      return oscar == null ? null : (Oscar) oscar;
      #endif
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

    private static void updateShopListings() {
      var definitions = getDefsFromDan();
      var oscar = grabAnOscar();

      if (oscar == null) {
        Logger.Error("couldn't find Oscar :C");
        return;
      }

      var listing = oscar.ShopInterface.Listings;

      for (var i = 0; i < listing.Count; i++) {
        if (listing[i].Item.ID == BED_ITEM_ID) {
          listing[i] = copyListing(definitions.locker, oscar.ShopInterface);
        }
      }

      listing.Add(copyListing(definitions.fertilizer, oscar.ShopInterface));
      listing.Add(copyListing(definitions.pgr, oscar.ShopInterface));
      listing.Add(copyListing(definitions.speedGrow, oscar.ShopInterface));
    }
  }
}
