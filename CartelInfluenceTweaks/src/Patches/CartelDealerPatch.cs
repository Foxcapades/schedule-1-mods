using Fxcpds;
using HarmonyLib;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

#if IL2CPP
using Il2CppInterop.Runtime;
using Il2CppScheduleOne.Cartel;
#elif MONO
using ScheduleOne.Cartel;
#endif

namespace CartelInfluenceTweaks.Patches {

  [HarmonyPatch(typeof(CartelDealer))]
  internal static class CartelDealerPatch {

    private static readonly ArrayList dealerActions = new ArrayList(8);

    [HarmonyPostfix]
    [HarmonyPatch("Start")]
    static void StartPostfix(CartelDealer __instance) {
      #if !RELEASE
      FxMod.Instance.LoggerInstance.Debug("CartelDealer.Start.Postfix({0})", __instance);
      #endif

      dealerActions.Add(new DealerActions(__instance, dealerActions.Remove));
    }

    internal class DealerActions: Component {
      private readonly CartelDealer dealer;
      private readonly Action<object> remover;
      private UnityAction? onDeal;
      private UnityAction? onDeath;

      internal DealerActions(CartelDealer dealer, Action<object> remover) {
        this.dealer = dealer;
        this.remover = remover;

        #if IL2CPP
        onDeal = DelegateSupport.ConvertDelegate<UnityAction>(onDealImpl);
        onDeath = DelegateSupport.ConvertDelegate<UnityAction>(onDeathImpl);
        #elif MONO
        onDeal = onDealImpl;
        onDeath = onDeathImpl;
        #endif

        dealer.onCompleteDeal.AddListener(onDeal);
        dealer.Health.onDieOrKnockedOut.AddListener(onDeath);
      }

      private void onDealImpl() {
        Mod.pushState(dealer.Region, State.CartelDeal);
        Cartel.Instance.Influence.ChangeInfluence(dealer.Region, 1f);
      }

      private void onDeathImpl() {
        dealer.onCompleteDeal.RemoveListener(onDeal);
        dealer.Health.onDieOrKnockedOut.RemoveListener(onDeath);

        onDeal = null;
        onDeath = null;

        remover.Invoke(this);
      }
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