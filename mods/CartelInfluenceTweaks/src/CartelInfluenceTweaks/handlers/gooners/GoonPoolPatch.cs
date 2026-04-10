using HarmonyLib;
using System;
using UnityEngine;

#if IL2CPP
using Il2CppScheduleOne.Cartel;
using Il2CppSystem.Collections.Generic;
#elif MONO
using ScheduleOne.Cartel;
using System.Collections.Generic;
#endif

namespace CartelInfluenceTweaks.handlers.gooners {

  [HarmonyPatch(typeof(GoonPool))]
  internal static class GoonPoolPatch {
    internal static Action<List<CartelGoon>>? onMultiGoonSpawn;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(GoonPool.SpawnMultipleGoons), new [] { typeof(Vector3), typeof(int), typeof(bool) })]
    private static void SpawnMultipleGoonsPostfix(List<CartelGoon> __result) {
      onMultiGoonSpawn?.Invoke(__result);
    }
  }
}