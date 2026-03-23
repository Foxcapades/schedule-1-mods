using HarmonyLib;
using MelonLoader;
#if IL2CPP
using Il2CppScheduleOne.Effects;
using Il2CppScheduleOne.Product;
using Il2CppSystem.Collections.Generic;
#elif MONO
using ScheduleOne.Effects;
using ScheduleOne.Product;
using System.Collections.Generic;
#endif

[assembly: MelonInfo(typeof(DonutMixFix.Mod), DonutMixFix.Mod.MOD_NAME, "1.0.0", "Foxcapades")]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace DonutMixFix {
  [HarmonyPatch(typeof(EffectMixCalculator), nameof(EffectMixCalculator.MixProperties))]
  public class Mod: MelonMod {
    public const string MOD_NAME = "DonutMixFix";
    private const string CalorieDenseEffectID = "caloriedense";

    // Sets __state to true if we are adding Explosive, and Calorie-Dense is
    // already on the target drug.
    //
    // The __state value we set here will be passed to Postfix.
    [HarmonyPrefix]
    static void Prefix(List<Effect> existingProperties, Effect newProperty, EDrugType drugType, out bool __state) {
      __state = false;
      if (newProperty.ID != CalorieDenseEffectID)
        return;

      foreach (var effect in existingProperties) {
        if (effect.ID == CalorieDenseEffectID) {
          __state = true;
          break;
        }
      }
    }

    [HarmonyPostfix]
    static List<Effect> Postfix(List<Effect> resultingEffects, bool __state) {
      // If __state was set to true by Prefix, then we should now have BOTH
      // Explosive and Calorie-Dense.  Remove Calorie-Dense and return the rest
      // of the effects.
      if (__state) {
        var newEffects = new List<Effect>(resultingEffects.Count - 1);

        foreach (var effect in resultingEffects) {
          if (effect.ID != CalorieDenseEffectID)
            newEffects.Add(effect);
        }

        return newEffects;
      }

      // __state was false, Calorie-Dense does not need to be removed.
      return resultingEffects;
    }
  }
}