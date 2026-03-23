using HarmonyLib;
using MelonLoader;
using MelonLoader.Preferences;
#if IL2CPP
using Il2CppInterop.Runtime;
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppSystem;
#elif MONO
using ScheduleOne.GameTime;
using ScheduleOne.PlayerScripts;
#endif

[assembly: MelonInfo(typeof(DayLengthModifier.Mod), DayLengthModifier.Mod.MOD_NAME, "1.0.0", "Foxcapades")]
[assembly: MelonGame("TVGS", "Schedule I")]

#nullable enable
namespace DayLengthModifier {
  public class Mod: MelonMod {
    public const string MOD_NAME = "DayLengthModifier";

    private const float MIN_MULTIPLIER = 0.1f;
    private const float MAX_MULTIPLIER = 100f;

    public override void OnInitializeMelon() {
      var validator = new Validator();
      var preferences = MelonPreferences.CreateCategory(MOD_NAME, "Day Length Modifier");

      modifier = preferences.CreateEntry(
        identifier: "modifier",
        display_name: "Day Length Multiplier",
        default_value: 1f,
        validator: validator
      );

#if IL2CPP
      Player.onPlayerSpawned += DelegateSupport.ConvertDelegate<Action<Player>>(action);
#elif MONO
      Player.onPlayerSpawned += action;
#endif
    }

    private static MelonPreferences_Entry<float>? modifier;

    public override void OnPreferencesSaved(string filepath) {
      setTheDangThing();
    }

    private static void action(Player _) {
      setTheDangThing();
    }

    private static void setTheDangThing() {
      TimeManager.Instance?.SetTimeSpeedMultiplier(1f/modifier!.Value);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(TimeManager), "Clean")]
    static void timeManagerClean() {
      setTheDangThing();
    }

    private class Validator: ValueValidator {
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
