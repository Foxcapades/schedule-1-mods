using Fxcpds;
using HarmonyLib;
using System.Collections;

#if IL2CPP
using Il2CppInterop.Runtime;
using Il2CppScheduleOne.Cartel;
#elif MONO
using ScheduleOne.Cartel;
#endif

namespace CartelInfluenceTweaks.Patches.Dealers {

  [HarmonyPatch(typeof(CartelDealer))]
  internal static class CartelDealerPatch {

    private static readonly ArrayList dealerActions = new ArrayList(8);

    [HarmonyPostfix]
    [HarmonyPatch("Start")]
    static void StartPostfix(CartelDealer __instance) {
      #if !RELEASE
      FxMod.Instance.LoggerInstance.Debug("CartelDealer.Start.Postfix({0})", __instance);
      #endif

      dealerActions.Add(new CartelDealerWatcher(__instance, dealerActions.Remove));
    }

    [HarmonyPrefix]
    [HarmonyPatch("DiedOrKnockedOut")]
    static void DiedPrefix(CartelDealer __instance) {
      #if !RELEASE
      FxMod.Instance.LoggerInstance.Debug("CartelDealer.DiedOrKnockedOut.Prefix({0})", nameof(__instance.Region));
      #endif
      if (Mod.shouldListen())
        Mod.pushState(__instance.Region, State.DealerDefeated);
    }

    [HarmonyPostfix]
    [HarmonyPatch("DiedOrKnockedOut")]
    static void DiedPostfix(CartelDealer __instance) {
      #if !RELEASE
      FxMod.Instance.LoggerInstance.Debug("CartelDealer.DiedOrKnockedOut.Postfix({0})", nameof(__instance.Region));
      #endif
      Mod.commonPostfix(__instance.Region);
    }
  }
}