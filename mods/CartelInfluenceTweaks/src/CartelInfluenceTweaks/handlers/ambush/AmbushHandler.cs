using HarmonyLib;
using Fxcpds;
using CartelInfluenceTweaks.handlers.gooners;

#if IL2CPP
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

using CounterList = System.Collections.Generic.List<CartelInfluenceTweaks.handlers.ambush.AmbushTracker>;

namespace CartelInfluenceTweaks.handlers.ambush {

  internal static class AmbushHandler {
    // ReSharper disable once CollectionNeverQueried.Local
    private static readonly CounterList activeAmbushes = new CounterList(2);

    /// <summary>
    /// Cartel goon list from the most recent ambush spawn.
    /// </summary>
    private static List<CartelGoon>? goonSquad;

    internal static void registerAmbush(Player target, List<CartelGoon> goons, EMapRegion region) =>
      activeAmbushes.Add(new AmbushTracker(region, target, goons, deregisterAmbush));

    private static void deregisterAmbush(AmbushTracker tracker) =>
      activeAmbushes.Remove(tracker);

    /// <summary>
    /// Ambush spawn callback used to collect the CartelGoon instances created
    /// for an ambush.
    /// </summary>
    private static void ambushSpawned(List<CartelGoon> ambushThugs) =>
      goonSquad = ambushThugs;

    [HarmonyPatch(typeof(Ambush))]
    private static class AmbushPatch {

      [HarmonyPrefix]
      [HarmonyPatch("SpawnAmbush")]
      private static void SpawnAmbushPrefix() {
        #if !RELEASE
        FxMod.Logger.Debug("ambush being spawned");
        #endif

        GoonPoolPatch.onMultiGoonSpawn = ambushSpawned;
      }

      [HarmonyPostfix]
      [HarmonyPatch("SpawnAmbush")]
      private static void SpawnAmbushPostfix(Ambush __instance, Player target) {
        #if !RELEASE
        FxMod.Logger.Debug(
          "registering cartel ambush targeting {0} in {1}",
          target.PlayerName,
          __instance.Region
        );
        #endif
        GoonPoolPatch.onMultiGoonSpawn = null;

        if (goonSquad == null)
          FxMod.Logger.Error("ambush goon list was null.  cannot track ambush");
        else
          registerAmbush(target, goonSquad, __instance.Region);
      }
    }
  }
}