using HarmonyLib;
using MelonLoader;
using UnityEngine;
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

    private static bool isInputKeyBased() {
      return Input.GetKeyDown(KeyCode.Alpha1)
        || Input.GetKeyDown(KeyCode.Alpha2)
        || Input.GetKeyDown(KeyCode.Alpha3)
        || Input.GetKeyDown(KeyCode.Alpha4)
        || Input.GetKeyDown(KeyCode.Alpha5)
        || Input.GetKeyDown(KeyCode.Alpha6)
        || Input.GetKeyDown(KeyCode.Alpha7)
        || Input.GetKeyDown(KeyCode.Alpha8)
        || Input.GetKeyDown(KeyCode.Alpha9)
        || Input.GetKeyDown(KeyCode.Alpha0);
    }
  }
}