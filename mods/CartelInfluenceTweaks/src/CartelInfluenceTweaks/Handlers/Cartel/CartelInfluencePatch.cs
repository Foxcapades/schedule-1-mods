using Fxcpds;
using HarmonyLib;

#if IL2CPP
using Il2CppScheduleOne.Cartel;
using Il2CppScheduleOne.Map;
#elif MONO
using ScheduleOne.Cartel;
using ScheduleOne.Map;
#endif

namespace CartelInfluenceTweaks.Handlers.Cartel {

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

      float changeAmount;

      switch (state) {
        case State.GraffitiCleaned:
          changeAmount = Mod.preferences.graffitiRemovalChange;
          break;
        case State.GraffitiCreated:
          changeAmount = Mod.preferences.graffitiCreationChange;
          break;
        case State.GraffitiInterrupted:
          changeAmount = Mod.preferences.graffitiInterruptedChange;
          break;
        case State.DealerDefeated:
          changeAmount = Mod.preferences.dealerKilledChange;
          break;
        case State.AmbushDefeated:
          changeAmount = Mod.preferences.ambushKilledChange;
          break;
        case State.CustomerStolen:
          changeAmount = Mod.preferences.customerGainedChange;
          break;

        // New features.
        case State.PlayerDeal:
          changeAmount = Mod.preferences.playerDealChange;
          break;
        case State.CartelGraffiti:
          changeAmount = Mod.preferences.cartelGraffitiChange;
          break;
        case State.CartelDeal:
          changeAmount = Mod.preferences.cartelDealChange;
          break;
        case State.PlayerProxyDeal:
          changeAmount = Mod.preferences.playerProxyDealChange;
          break;
        case State.PlayerDefeated:
          changeAmount = amount;
          break;
        default:
          FxMod.Instance.LoggerInstance.Error("unrecognized state " + state);
          return true;
      }

      changeAmount /= 100;

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