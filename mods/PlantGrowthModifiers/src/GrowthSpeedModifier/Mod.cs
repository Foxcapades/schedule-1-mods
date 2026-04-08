using Fxcpds;
using HarmonyLib;
using MelonLoader;
#if IL2CPP
using Il2CppScheduleOne.Growing;
using Il2CppScheduleOne.ItemFramework;
#elif MONO
using ScheduleOne.Growing;
using ScheduleOne.ItemFramework;
#endif

[assembly: MelonInfo(typeof(GrowthSpeedModifier.Mod), GrowthSpeedModifier.Mod.MOD_NAME, "1.0.1", "Foxcapades")]
[assembly: MelonGame("TVGS", "Schedule I")]

#nullable enable
namespace GrowthSpeedModifier {
  public class Mod: MelonMod {
    public const string MOD_NAME = "Growth Speed Modifiers";

    private static MelonPreferences_Entry<float>? cocaModifier;
    private static MelonPreferences_Entry<float>? shroomModifier;
    private static MelonPreferences_Entry<float>? weedModifier;

    public override void OnInitializeMelon() {
      var validator   = new NumberValidator<float>(0.1f, 50f);
      var preferences = MelonPreferences.CreateCategory(MOD_NAME, "Modifiers");

      cocaModifier = preferences.CreateEntry(
        identifier: "cocaModifier",
        display_name: "Coca Growth Multiplier",
        default_value: 1f,
        validator: validator
      );
      shroomModifier = preferences.CreateEntry(
        identifier: "shroomModifier",
        display_name: "Mushroom Growth Multiplier",
        default_value: 1f,
        validator: validator
      );
      weedModifier = preferences.CreateEntry(
        identifier: "weedModifier",
        display_name: "Weed Growth Multiplier",
        default_value: 1f,
        validator: validator
      );
    }

    private static float getModifier(Plant plant) {
      switch (plant.SeedDefinition.ID) {
        case "cocaseed":
          return cocaModifier!.Value;

        case "granddaddypurpleseed":
        case "greencrackseed":
        case "ogkushseed":
        case "sourdieselseed":
          return weedModifier!.Value;

        default:
          return 1f;
      }
    }

    [HarmonyPatch(typeof(ShroomColony), "SetGrowthPercentage")]
    private static class ShroomColonyPatch {
      static void Prefix(ref float percent, ShroomColony __instance) {
        if (__instance.GrowthProgress == 0)
          return;

        var delta = percent - __instance.GrowthProgress;
        percent = __instance.GrowthProgress + delta * shroomModifier!.Value;
      }
    }

    [HarmonyPatch(typeof(Plant))]
    private static class PlantPatch {
      private static bool growth;

      [HarmonyPrefix]
      [HarmonyPatch(nameof(Plant.SetNormalizedGrowthProgress))]
      static void SetNormalizedGrowthProgressPrefix(ref float progress, Plant __instance) {
        if (growth) {
          growth = false;
          return;
        }

        if (__instance.NormalizedGrowthProgress == 0)
          return;

        var delta = progress - __instance.NormalizedGrowthProgress;
        progress = __instance.NormalizedGrowthProgress + delta * getModifier(__instance);
      }

      [HarmonyPrefix]
      [HarmonyPatch(nameof(Plant.AdditiveApplied))]
      static void AdditiveAppliedPrefix(AdditiveDefinition additive, bool isInitialApplication) {
        if (isInitialApplication && additive.InstantGrowth > 0f) {
          growth = true;
        }
      }
    }
  }
}
