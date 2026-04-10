using Fxcpds;
using HarmonyLib;

#if IL2CPP
using Il2CppScheduleOne.Cartel;
using Il2CppScheduleOne.Map;
#elif MONO
using ScheduleOne.Cartel;
using ScheduleOne.Map;
#endif

namespace CartelInfluenceTweaks.handlers.cartel {

  [HarmonyPatch(
    typeof(CartelInfluence),
    nameof(CartelInfluence.ChangeInfluence),
    new[] {typeof(EMapRegion), typeof(float)}
  )]
  internal class CartelInfluencePatch {

    static bool Prefix(EMapRegion region, ref float amount) {
      if (!Mod.shouldListen()) {
        #if !RELEASE
        FxMod.Instance.LoggerInstance.Debug("cartel influence patch: not listening");
        #endif

        return false;
      }

      #if !RELEASE
      FxMod.Instance.LoggerInstance.Debug(
        "CartelInfluence.ChangeInfluence.Prefix(regin: {0}, amount: {1})",
        region,
        amount
        );
      #endif

      var (state, found) = Mod.popState(region);

      if (!found) {
        FxMod.Instance.LoggerInstance.Error("cartel influence changed but no multiplier was queued!");
        return true;
      }

      var changeAmount = Mod.preferences[state.type] / 100 * state.sign;

      #if !RELEASE
      FxMod.Instance.LoggerInstance.Debug(
        "replacing {0} cartel influence change {1} with {2}",
        region,
        amount,
        changeAmount
      );
      #endif

      amount = changeAmount;

      return amount != 0;
    }
  }
}