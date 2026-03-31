using MelonLoader;

#nullable enable
namespace OscarsInventoryTweaks {
  internal sealed class Preferences {
    private readonly MelonPreferences_Entry<bool> sellFertilizer;
    private readonly MelonPreferences_Entry<bool> sellPGR;
    private readonly MelonPreferences_Entry<bool> sellSpeedGrow;
    private readonly MelonPreferences_Entry<bool> sellBigSprinkler;
    private readonly MelonPreferences_Entry<bool> sellLocker;
    private readonly MelonPreferences_Entry<bool> sellShelves;
    private readonly MelonPreferences_Entry<bool> sellClosets;

    public bool SellFertilizer => sellFertilizer.Value;
    public bool SellPgr => sellPGR.Value;
    public bool SellSpeedGrow => sellSpeedGrow.Value;
    public bool SellBigSprinkler => sellBigSprinkler.Value;
    public bool SellLocker => sellLocker.Value;
    public bool SellShelves => sellShelves.Value;
    public bool SellClosets => sellClosets.Value;

    public Preferences(string configPath) {
      var growth = MelonPreferences.CreateCategory(Mod.MOD_ID + " - Additives", "Growth Additives");
      growth.SetFilePath(configPath);
      sellFertilizer   = growth.CreateEntry("sellFertilizer", true, "Sell Fertilizer");
      sellPGR          = growth.CreateEntry("sellPGR", true, "Sell PGR");
      sellSpeedGrow    = growth.CreateEntry("sellSpeedGrow", true, "Sell Speed Grow");

      var equipment = MelonPreferences.CreateCategory(Mod.MOD_ID + " - Growing", "Equipment");
      sellBigSprinkler = equipment.CreateEntry("sellBigSprinkler", true, "Sell Big Sprinkler");

      var storage = MelonPreferences.CreateCategory(Mod.MOD_ID + " - Storage", "Storage");
      storage.SetFilePath(configPath);
      sellLocker  = storage.CreateEntry("sellLocker", true, "Sell Locker");
      sellShelves = storage.CreateEntry("sellShelves", true, "Sell Shelves");
      sellClosets = storage.CreateEntry("sellClosets", true, "Sell Closets");
    }
  }
}