using Fxcpds;
using HarmonyLib;

#if IL2CPP
using Il2CppScheduleOne.Cartel;
using Il2CppScheduleOne.Map;
#elif MONO
using ScheduleOne.Cartel;
using ScheduleOne.Map;
#endif

namespace CartelInfluenceTweaks.Patches {
  [HarmonyPatch(typeof(CartelInfluence), nameof(CartelInfluence.ChangeInfluence), typeof(EMapRegion), typeof(float))]
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

      float multiplier = 1f;

      switch (state) {
        case State.GraffitiCleaned:
          multiplier = Mod.preferences.graffitiRemoved;
          break;
        case State.GraffitiCreated:
          multiplier = Mod.preferences.graffitiCreated;
          break;
        case State.GraffitiInterrupted:
          multiplier = Mod.preferences.graffitiInterrupted;
          break;
        case State.DealerDefeated:
          multiplier = Mod.preferences.killedDealer;
          break;
        case State.AmbushDefeated:
          multiplier = Mod.preferences.killedAmbush;
          break;
        case State.CustomerStolen:
          multiplier = Mod.preferences.customerGained;
          break;

        // New features.
        case State.PlayerDeal:
          multiplier = Mod.preferences.playerDeal / 100;
          break;
        case State.CartelGraffiti:
          multiplier = Mod.preferences.cartelGraffiti / 100;
          break;
        case State.CartelDeal:
          multiplier = Mod.preferences.cartelDeal / 100;
          break;
        default:
          FxMod.Instance.LoggerInstance.Error("unrecognized state " + state);
          break;
      }

      var newAmount = amount * multiplier;

      #if !RELEASE
      FxMod.Instance.LoggerInstance.Debug("applying multiplier {0} to {1} influence change {2} = {3}", multiplier, region, amount, newAmount);
      #endif

      amount = newAmount;

      return amount != 0;
    }
  }
}