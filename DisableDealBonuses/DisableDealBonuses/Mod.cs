using HarmonyLib;
using MelonLoader;
#if IL2CPP
using Il2CppScheduleOne.Economy;
#elif MONO
using ScheduleOne.Economy;
#endif

[assembly: MelonInfo(typeof(DisableDealBonuses.Mod), DisableDealBonuses.Mod.MOD_NAME, "1.0.0", "Foxcapades")]
[assembly: MelonGame("TVGS", "Schedule I")]

#nullable enable
namespace DisableDealBonuses {
  public class Mod: MelonMod {
    public const string MOD_NAME = "DisableDealBonuses";

    public override void OnInitializeMelon() {
    }

    [HarmonyPatch(typeof(Customer), nameof(Customer.ProcessHandover))]
    private static class CustomerPatch {
      // ReSharper disable once RedundantAssignment
      static void Prefix(ref bool giveBonuses) {
        giveBonuses = false;
      }
    }
  }
}
