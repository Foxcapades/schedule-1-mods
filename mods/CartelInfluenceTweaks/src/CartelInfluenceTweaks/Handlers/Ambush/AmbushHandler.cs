using Fxcpds;
using HarmonyLib;
using UnityEngine;

#if IL2CPP
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppScheduleOne.Cartel;
using Il2CppScheduleOne.Map;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppSystem.Collections.Generic;
#elif MONO
using ScheduleOne.Cartel;
using ScheduleOne.Map;
using ScheduleOne.PlayerScripts;
using System.Collections.Generic;
#endif

using CounterList = System.Collections.Generic.List<CartelInfluenceTweaks.Handlers.Ambush.AmbushTracker>;

namespace CartelInfluenceTweaks.Handlers.Ambush {

  internal static class AmbushHandler {
    // ReSharper disable once CollectionNeverQueried.Local
    private static readonly CounterList activeAmbushes = new CounterList(2);

    internal static void registerAmbush(
      Player target,
      List<CartelGoon> goons,
      EMapRegion region
    ) {
      activeAmbushes.Add(new AmbushTracker(region, target, goons, deregisterAmbush));
    }

    private static void deregisterAmbush(AmbushTracker tracker) {
      activeAmbushes.Remove(tracker);
    }

    [HarmonyPatch(typeof(Il2CppScheduleOne.Cartel.Ambush))]
    private static class AmbushPatch {
      [HarmonyPostfix]
      [HarmonyPatch("SpawnAmbush", typeof(Player), typeof(Il2CppStructArray<Vector3>))]
      static void SpawnAmbush(
        Il2CppScheduleOne.Cartel.Ambush __instance,
        List<CartelGoon> __result,
        Player target,
        Vector3[] potentialSpawnPoints
      ) {
        #if !RELEASE
        FxMod.Instance.LoggerInstance.Debug(
          "registering cartel ambush targeting {0} in {1}",
          target.PlayerName,
          __instance.Region
        );
        #endif

        registerAmbush(target, __result, __instance.Region);
      }
    }
  }
}