using HarmonyLib;
using System;
#if IL2CPP
using Il2CppScheduleOne.NPCs;
#elif MONO
using ScheduleOne.NPCs;
#endif

namespace CartelInfluenceTweaks.Patches.Ambush {

  [HarmonyPatch(typeof(NPC))]
  internal static class NPCPatch {
    internal static event Action<NPC> onUnconscious;
    [HarmonyPostfix]
    [HarmonyPatch(nameof(NPC.IsConscious), MethodType.Getter)]
    static void IsConsciousPostfix(NPC __instance, bool __result) {
      if (!__result)
        onUnconscious.Invoke(__instance);
    }
  }
}