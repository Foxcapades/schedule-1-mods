using HarmonyLib;
using System;

#if IL2CPP
using Il2CppScheduleOne.NPCs;
#elif MONO
using ScheduleOne.NPCs;
#endif

namespace CartelInfluenceTweaks.handlers.npcs {
  internal static class NPCHealthHandler {
    internal static event Action<NPC>? onDie;
    internal static event Action<NPC>? onKnockout;
    internal static event Action<NPC>? onRevive;
    internal static event Action<NPC>? onDieOrKnockout;

    [HarmonyPatch(typeof(NPCHealth))]
    private static class NPCHealthPatch {

      [HarmonyPostfix]
      [HarmonyPatch(nameof(NPCHealth.Die))]
      private static void OnDie(NPCHealth __instance) {
        if (!__instance.IsDead)
          return;

        var npc = __instance.GetComponent<NPC>();

        onDie?.Invoke(npc);
        onDieOrKnockout?.Invoke(npc);
      }

      [HarmonyPostfix]
      [HarmonyPatch(nameof(NPCHealth.KnockOut))]
      private static void OnKnockout(NPCHealth __instance) {
        if (!__instance.IsKnockedOut)
          return;

        var npc = __instance.GetComponent<NPC>();

        onKnockout?.Invoke(npc);
        onDieOrKnockout?.Invoke(npc);
      }

      [HarmonyPostfix]
      [HarmonyPatch(nameof(NPCHealth.Revive))]
      private static void OnRevive(NPCHealth __instance) {
        var npc = __instance.GetComponent<NPC>();

        onRevive?.Invoke(npc);
      }
    }
  }
}