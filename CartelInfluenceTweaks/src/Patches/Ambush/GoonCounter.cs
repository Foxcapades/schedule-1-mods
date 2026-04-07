using Fxcpds;
using Il2CppInterop.Runtime.Runtime;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

#if IL2CPP
using Il2CppInterop.Runtime;
using Il2CppScheduleOne.Cartel;
using Il2CppScheduleOne.NPCs;
#endif

namespace CartelInfluenceTweaks.Patches.Ambush {
  internal class GoonCounter {
    private List<CartelGoon>? goons;
    private UnityAction? action;
    private Action<object>? remover;

    internal int remaining { get; private set; }

    internal GoonCounter(List<CartelGoon> goons, Action<object> remover) {
      this.goons = goons;
      this.remaining = goons.Count;
      this.remover = remover;
      #if IL2CPP
      this.action = DelegateSupport.ConvertDelegate<UnityAction>(onKnockout);
      #elif MONO
      this.action = new UnityAction(onKnockout);
      #endif

      #if !RELEASE
      FxMod.Instance.LoggerInstance.Debug("registering new goon counter");
      #endif

      this.register();
    }

    private void onKnockout() {
      if (--remaining <= 0) {
        deregister();
        return;
      }

      #if !RELEASE
      FxMod.Instance.LoggerInstance.Debug("{0} goons remaining in pool", remaining);
      #endif
    }

    private void onLastGoon() {

    }

    private void onFinalKnockout(NPC npc) {
      var goon = Interop.cast<CartelGoon>(npc);

      if (goon == null)
        return;
    }

    private void register() {
      foreach (var goon in goons!)
        goon.Health.onDieOrKnockedOut.AddListener(action);
    }

    private void deregister() {
      if (goons == null) {
        FxMod.Instance.LoggerInstance.Error("attempted to deregister goon pool more than once");
        return;
      }

      Mod.pushState(goons[0].Region, State.AmbushDefeated);

      #if !RELEASE
      FxMod.Instance.LoggerInstance.Debug("deregistering goon counter");
      #endif

      foreach (var goon in goons) {
        goon.Health.onDieOrKnockedOut.RemoveListener(action);
      }

      remover!(this);

      goons = null;
      action = null;
      remover = null;
    }
  }
}