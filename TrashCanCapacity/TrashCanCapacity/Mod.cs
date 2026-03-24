using HarmonyLib;
using MelonLoader;
#if IL2CPP
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.ObjectScripts;
#elif MONO
using ScheduleOne.ItemFramework;
using ScheduleOne.ObjectScripts;
#endif

[assembly: MelonInfo(typeof(TrashCanCapacity.Mod), TrashCanCapacity.Mod.MOD_NAME, "1.0.0", "Foxcapades")]
[assembly: MelonGame("TVGS", "Schedule I")]

#nullable enable
namespace TrashCanCapacity {
  public class Mod: MelonMod {
    public const string MOD_NAME = "TrashCanCapacity";

#if DEBUG
    private static void debugLog(string message) {
      Melon<Mod>.Logger.Msg(message);
    }
#endif

    [HarmonyPatch(typeof(TrashContainerItem), nameof(TrashContainerItem.InitializeGridItem))]
    private static class Patch {
      static void Prefix(ItemInstance instance, TrashContainerItem __instance) {
        if (instance.ID == "trashcan") {
#if DEBUG
          debugLog("updating newly placed trashcan capacity");
#endif
          __instance.Container.TrashCapacity *= 2;
        }
      }
    }
  }
}
