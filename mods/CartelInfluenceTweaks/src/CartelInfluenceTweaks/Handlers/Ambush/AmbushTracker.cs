using Fxcpds;
using System;
using UnityEngine.Events;

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

namespace CartelInfluenceTweaks.Handlers.Ambush {

  /// <summary>
  /// Tracks cartel goons remaining in an ambush.
  /// </summary>
  internal class AmbushTracker {
    #if IL2CPP

    private readonly IntPtr[] goons;

    private UnityAction? onPlayerDiedAction;

    #elif MONO

    private readonly CartelGoon?[] goons;
    #endif

    private readonly Action<AmbushTracker> onFinalize;

    internal readonly EMapRegion region;

    internal readonly Player target;

    internal int remaining { get; private set; }

    internal AmbushTracker(
      EMapRegion region,
      Player target,
      List<CartelGoon> goons,
      Action<AmbushTracker> onFinalize
    ) {
      #if IL2CPP
      this.goons = new IntPtr[goons.Count];
      #elif MONO
      this.goons = new CartelGoon[goons.Count];
      #endif

      this.onFinalize = onFinalize;
      this.region     = region;
      this.target     = target;
      this.remaining  = goons.Count;

      for (var i = 0; i < goons.Count; i++) {
        #if IL2CPP
        this.goons[i] = goons[i].Pointer;
        #elif MONO
        this.goons[i] = goons[i];
        #endif
      }

      #if !RELEASE
      FxMod.Instance.LoggerInstance.Debug("tracking {0} goons", remaining);
      #endif
    }

    private void register() {
      Cartel.CartelGoonHandler.onDefeated += onGoonKnockout;

      #if IL2CPP
      #elif MONO
      target.Health.onDie.AddListener(onPlayerDied);
      #endif
    }

    internal void finalize() {
      Cartel.CartelGoonHandler.onDefeated -= onGoonKnockout;

      #if IL2CPP
      #elif MONO
      target.Health.onDie.RemoveListener(onPlayerDied);
      #endif

      onFinalize(this);
    }

    private void onPlayerDied() {
      Mod.pushState(region, State.PlayerDefeated);
      finalize();
    }

    private void onGoonKnockout(CartelGoon goon) {
      for (var i = 0; i < goons.Length; i++) {
        if (interopGoonEquals(goon, goons[i]))
          goons[i] = default;
        remaining--;

        #if !RELEASE
        FxMod.Instance.LoggerInstance.Debug("ambush goon defeated");
        #endif
      }

      #if !RELEASE
      FxMod.Instance.LoggerInstance.Debug("{0} goons remaining in ambush", remaining);
      #endif

      if (remaining > 0)
        return;

      Mod.pushState(region, State.AmbushDefeated);
      finalize();
    }

    #if IL2CPP
    private static bool interopGoonEquals(CartelGoon g1, IntPtr g2) {
      return g1.Pointer == g2;
    }
    #elif MONO
    private static bool interopGoonEquals(CartelGoon g1, CartelGoon g2) {
      return ReferenceEquals(goon, goons[i]);
    }
    #endif
  }
}