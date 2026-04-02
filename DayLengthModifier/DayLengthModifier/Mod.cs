using System.Diagnostics;
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
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.Default)]

#nullable enable
namespace DayLengthModifier {
  public class Mod: FxMod {
    public const string MOD_NAME = "Adjustable Day Length";

    private const float MIN_MULTIPLIER = 0.1f;
    private const float MAX_MULTIPLIER = 60f;

    private static MelonPreferences_Entry<float>? modifier;

    public override void OnInitializeMelon() {
      var preferences = MelonPreferences.CreateCategory("Day Length Modifier", "Day Length Modifier");

      modifier = preferences.CreateEntry(
        identifier: "modifier",
        display_name: "Day Length Multiplier",
        description: "Valid range: 0.1 = 10x faster days - 60 = real world time",
        default_value: 1f,
        validator: new NumberValidator<float>(MIN_MULTIPLIER, MAX_MULTIPLIER)
      );
    }

    protected override void onPlayerLoaded(Player _) {
      TimeManagerClean();
    }

    protected override void onModPreferencesSaved() {
      TimeManagerClean();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(TimeManager), "Clean")]
    private static void TimeManagerClean() {
      TimeManager.Instance?.SetTimeSpeedMultiplier(1f/modifier!.Value);
    }
  }
}
