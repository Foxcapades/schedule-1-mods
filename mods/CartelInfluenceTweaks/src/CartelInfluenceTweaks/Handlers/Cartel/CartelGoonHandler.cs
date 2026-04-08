using Fxcpds;
using System;

#if IL2CPP
using Il2CppScheduleOne.Cartel;
using Il2CppScheduleOne.NPCs;
#elif MONO
using ScheduleOne.Cartel;
using ScheduleOne.NPCs;
#endif

namespace CartelInfluenceTweaks.Handlers.Cartel {
  internal static class CartelGoonHandler {
    internal static event Action<CartelGoon>? onDefeated;

    static CartelGoonHandler() {
      NPCs.NPCHealthHandler.onDieOrKnockout += onDieOrKnockout;
    }

    private static void onDieOrKnockout(NPC npc) {
      var goon = Interop.cast<CartelGoon>(npc);
      if (goon != null)
        onDefeated?.Invoke(goon);
    }
  }
}