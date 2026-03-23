using HarmonyLib;
using MelonLoader;
using MelonLoader.Preferences;
#if IL2CPP
using Il2CppScheduleOne.Growing;
#elif MONO
using ScheduleOne.Growing;
#endif

[assembly: MelonInfo(typeof(GrowthSpeedModifier.Mod), GrowthSpeedModifier.Mod.MOD_NAME, "1.0.0", "Foxcapades")]
[assembly: MelonGame("TVGS", "Schedule I")]

#nullable enable
namespace GrowthSpeedModifier {
  public class Mod: MelonMod {
    public const string MOD_NAME = "GrowthSpeedModifier";

    private const float MIN_MULTIPLIER = 0.1f;
    private const float MAX_MULTIPLIER = 50f;

    private static MelonPreferences_Entry<float>? cocaModifier;
    private static MelonPreferences_Entry<float>? shroomModifier;
    private static MelonPreferences_Entry<float>? weedModifier;

    public override void OnInitializeMelon() {
      var validator = new Validator();
      var preferences = MelonPreferences.CreateCategory(MOD_NAME, "Growth Speed Modifiers");

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

    [HarmonyPatch(typeof(ShroomColony), "SetGrowthPercentage")]
    private static class ShroomColonyPatch {
      static void Prefix(ref float percent, ShroomColony __instance) {
        if (__instance.GrowthProgress == 0)
          return;

        var delta = percent - __instance.GrowthProgress;
        percent = __instance.GrowthProgress + delta * shroomModifier!.Value;
      }
    }

    [HarmonyPatch(typeof(Plant), nameof(Plant.SetNormalizedGrowthProgress))]
    private static class PlantPatch {
      static void Prefix(ref float progress, Plant __instance) {
        if (__instance.NormalizedGrowthProgress == 0)
          return;

        var delta = progress - __instance.NormalizedGrowthProgress;
        progress = __instance.NormalizedGrowthProgress + delta * getModifier(__instance);
      }
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

    private sealed class Validator: ValueValidator {
      public override bool IsValid(object value) {
        var fValue = (float)value;
        return fValue >= MIN_MULTIPLIER && fValue <= MAX_MULTIPLIER;
      }

      public override object EnsureValid(object value) {
        var fValue = (float)value;

        if (fValue < 0.1f)
          return 0.1f;

        if (fValue > 50f)
          return 50f;

        return fValue;
      }
    }
  }
}
