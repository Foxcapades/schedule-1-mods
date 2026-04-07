using System;
using ScheduleOne.Cartel;
using UnityEngine.Events;

namespace CartelInfluenceTweaks.Patches.Dealers {
  internal class CartelDealerWatcher {
    private readonly CartelDealer dealer;
    private readonly Action<object> remover;
    private UnityAction? onDeal;
    private UnityAction? onDeath;

    internal CartelDealerWatcher(CartelDealer dealer, Action<object> remover) {
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
}