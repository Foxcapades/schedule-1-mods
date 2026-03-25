using HarmonyLib;
using MelonLoader;
#if IL2CPP
using Il2CppScheduleOne;
using Il2CppScheduleOne.PlayerScripts;
#elif MONO
using ScheduleOne;
using ScheduleOne.PlayerScripts;
#endif

[assembly: MelonInfo(typeof(DisableHotbarScroll.Mod), DisableHotbarScroll.Mod.MOD_NAME, "1.0.0", "Foxcapades")]
[assembly: MelonGame("TVGS", "Schedule I")]

#nullable enable
namespace DisableHotbarScroll {
  public class Mod: MelonMod {
    public const string MOD_NAME = "Disable Hotbar Scroll";

    [HarmonyPatch(typeof(PlayerInventory), "UpdateHotbarSelection")]
    private static class Patch {
      static bool Prefix() {
        return GameInput.MouseScrollDelta == 0;
      }
    }
  }
}