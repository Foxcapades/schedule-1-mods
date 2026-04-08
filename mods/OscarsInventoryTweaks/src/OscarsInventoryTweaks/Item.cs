#nullable enable
namespace OscarsInventoryTweaks {
  internal readonly struct Category {
    public static readonly Category Additives = new Category("Additives", "extralonglifesoil");
    public static readonly Category Storage   = new Category("Storage", "bed");
    public static readonly Category Equipment = new Category("Growing", "soilpourer");

    public static readonly Category[] allCategories = {
      Additives,
      Storage,
      Equipment,
    };

    public readonly string name;
    public readonly string appearsAfter;

    private Category(string name, string appearsAfter) {
      this.name = name;
      this.appearsAfter = appearsAfter;
    }

    public override bool Equals(object? obj) {
      return obj != null && (obj as Category?)?.name == this.name;
    }

    public override int GetHashCode() {
      return name.GetHashCode();
    }

    public static bool operator ==(Category a, Category b) =>
      a.name == b.name;

    public static bool operator !=(Category a, Category b) =>
      a.name != b.name;
  }

  internal readonly struct Item {
    public static readonly Item Fertilizer = new Item("fertilizer", Category.Additives);
    public static readonly Item PGR        = new Item("pgr", Category.Additives);
    public static readonly Item SpeedGrow  = new Item("speedgrow", Category.Additives);

    public static readonly Item Locker = new Item("locker", Category.Storage);

    public static readonly Item SmallStorageCloset  = new Item("smallstoragecloset", Category.Storage);
    public static readonly Item MediumStorageCloset = new Item("mediumstoragecloset", Category.Storage);
    public static readonly Item LargeStorageCloset  = new Item("largestoragecloset", Category.Storage);
    public static readonly Item HugeStorageCloset   = new Item("hugestoragecloset", Category.Storage);

    public static readonly Item SmallStorageRack  = new Item("smallstoragerack", Category.Storage);
    public static readonly Item MediumStorageRack = new Item("mediumstoragerack", Category.Storage);
    public static readonly Item LargeStorageRack  = new Item("largestoragerack", Category.Storage);

    public static readonly Item BigSprinkler   = new Item("bigsprinkler", Category.Equipment);
    public static readonly Item AirConditioner = new Item("acunit", Category.Equipment);

    public static int itemCount => allItems.Length;
    public static readonly Item[] allItems = {
      Fertilizer,
      PGR,
      SpeedGrow,

      Locker,

      BigSprinkler,
      AirConditioner,

      SmallStorageCloset,
      MediumStorageCloset,
      LargeStorageCloset,
      HugeStorageCloset,

      SmallStorageRack,
      MediumStorageRack,
      LargeStorageRack,
    };

    public readonly string id;
    public readonly Category category;

    private Item(string id, Category category) {
      this.id = id;
      this.category = category;
    }
  }
}