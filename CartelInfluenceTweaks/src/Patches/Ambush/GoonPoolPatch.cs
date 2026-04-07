using Fxcpds;
using HarmonyLib;
using System.Collections;

#if IL2CPP
using Il2CppInterop.Runtime;
using Il2CppScheduleOne.Cartel;
using Il2CppSystem.Collections.Generic;
#elif MONO
using ScheduleOne.Cartel;
using System.Collections.Generic;
#endif

namespace CartelInfluenceTweaks.Patches.Ambush {

  [HarmonyPatch(typeof(GoonPool))]
  internal class GoonPoolPatch {
    private static readonly ArrayList counters = new ArrayList();

    [HarmonyPostfix]
    [HarmonyPatch(nameof(GoonPool.SpawnMultipleGoons))]
    static void SpawnMultipleGoonsPostfix(List<CartelGoon> __result) {
      #if !RELEASE
      FxMod.Instance.LoggerInstance.Debug("GoonPool.SpawnMultipleGoons.Postfix(goonCount = {0})", __result.Count);
      #endif

      counters.Add(new GoonCounter(__result, counters.Remove));
    }
  }
}