using Fxcpds;
using HarmonyLib;
using MelonLoader;
#if IL2CPP
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.PlayerScripts;
#elif MONO
using ScheduleOne.GameTime;
using ScheduleOne.PlayerScripts;
#endif

[assembly: MelonInfo(typeof(DayLengthModifier.Mod), DayLengthModifier.Mod.MOD_NAME, "1.0.0", "Foxcapades")]
[assembly: MelonGame("TVGS", "Schedule I")]

#nullable enable
namespace DayLengthModifier {
  public class Mod: FxMod {
    public const string MOD_NAME = "DayLengthModifier";

    private const float MIN_MULTIPLIER = 0.1f;
    private const float MAX_MULTIPLIER = 100f;

    private static MelonPreferences_Entry<float>? modifier;

    public override void OnInitializeMelon() {
      var preferences = MelonPreferences.CreateCategory(MOD_NAME, "Day Length Modifier");

      modifier = preferences.CreateEntry(
        identifier: "modifier",
        display_name: "Day Length Multiplier",
        default_value: 1f,
        validator: new NumberValidator<float>(MIN_MULTIPLIER, MAX_MULTIPLIER)
      );
    }

    protected override void onPlayerLoaded(Player _) {
      setTheDangThing();
    }

    protected override void onModPreferencesSaved() {
      setTheDangThing();
    }

    private static void setTheDangThing() {
      TimeManager.Instance?.SetTimeSpeedMultiplier(1f/modifier!.Value);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(TimeManager), "Clean")]
    private static void TimeManagerClean() {
      setTheDangThing();
    }
  }
}
