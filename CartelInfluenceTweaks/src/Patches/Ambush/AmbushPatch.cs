using Fxcpds;
using HarmonyLib;

#if IL2CPP
using Iter = Il2CppScheduleOne.Cartel.Ambush.__c__DisplayClass12_0.ObjectCompilerGeneratedNPrivateSealedIEnumerator1ObjectIEnumeratorIDisposableInObObObUnique;
#elif MONO
using ScheduleOne.Cartel;
using ScheduleOne.PlayerScripts;
using UnityEngine;
#endif

namespace CartelInfluenceTweaks.Patches.Ambush {
  internal static class AmbushPatch {

    #if IL2CPP
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Iter), nameof(Iter.MoveNext))]
    static void MoveNextPostfix(Iter __instance, bool __result) {
      #if !RELEASE
      FxMod.Instance.LoggerInstance.Debug("AmbushPatch.Generator.Postfix({0})", __result);
      #endif

      if (__result)
        return;

      Mod.pushState(__instance.__4__this.__4__this.region, State.AmbushDefeated);
    }
    #elif MONO
    [HarmonyReversePatch]
    [HarmonyPatch(typeof(Ambush), "SpawnAmbush", typeof(Player), typeof(Vector3[]))]
    static void SpawnAmbush(Player target, Vector3[] potentialSpawnPoints) {

    }
    #endif
  }
}
