using Fxcpds;
using HarmonyLib;

#if IL2CPP
using Il2CppScheduleOne.Graffiti;
#elif MONO
using ScheduleOne.Graffiti;
#endif

namespace CartelInfluenceTweaks.Patches {
  [HarmonyPatch(typeof(WorldSpraySurface))]
  internal static class WorldSpraySurfacePatch {
    [HarmonyPrefix]
    [HarmonyPatch("Reward")]
    static void RewardPrefix(WorldSpraySurface __instance) {
      #if !RELEASE
      FxMod.Instance.LoggerInstance.Debug("WorldSpraySurfacePatch.RewardPrefix({0})", __instance.Region);
      #endif
      if (Mod.shouldListen())
        Mod.pushState(__instance.Region, State.GraffitiCreated);
    }

    [HarmonyPostfix]
    [HarmonyPatch("Reward")]
    static void RewardPostfix(WorldSpraySurface __instance) {
      #if !RELEASE
      FxMod.Instance.LoggerInstance.Debug("WorldSpraySurfacePatch.RewardPostfix({0})", __instance.Region);
      #endif
      Mod.commonPostfix(__instance.Region);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(WorldSpraySurface.CleanGraffiti))]
    static void CleanPrefix(WorldSpraySurface __instance) {
      #if !RELEASE
      FxMod.Instance.LoggerInstance.Debug("WorldSpraySurfacePatch.CleanPrefix({0})", __instance.Region);
      #endif
      if (Mod.shouldListen())
        Mod.pushState(__instance.Region, State.GraffitiCleaned);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(WorldSpraySurface.CleanGraffiti))]
    static void CleanPostfix(WorldSpraySurface __instance) {
      #if !RELEASE
      FxMod.Instance.LoggerInstance.Debug("WorldSpraySurfacePatch.CleanPostfix({0})", __instance.Region);
      #endif
      Mod.commonPostfix(__instance.Region);
    }
  }
}