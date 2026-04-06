using Fxcpds;
using HarmonyLib;
using MelonLoader;
using System.Diagnostics;

#if IL2CPP
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.PlayerScripts;
#elif MONO
using ScheduleOne.GameTime;
using ScheduleOne.PlayerScripts;
#endif

[assembly: Debuggable(DebuggableAttribute.DebuggingModes.Default)]
[assembly: MelonGame("TVGS", "Schedule I")]
[assembly: MelonInfo(typeof(DayLengthModifier.Mod), DayLengthModifier.Mod.MOD_NAME, "1.0.2", "Foxcapades")]

namespace DayLengthModifier {
  public class Mod: FxMod {
    public const string MOD_NAME = "Day Length Modifier";

    private const float MIN_MULTIPLIER = 0.1f;
    private const float MAX_MULTIPLIER = 60f;

    private static MelonPreferences_Entry<float>? modifier;

    public override void OnInitializeMelon() {
      var preferences = MelonPreferences.CreateCategory(MOD_NAME, MOD_NAME);

      (modifier = preferences.CreateEntry(
        identifier: "modifier",
        display_name: "Day Length Multiplier",
        description: "Valid range: 0.1 = 10x faster days - 60 = real world time",
        default_value: 1f,
        validator: new NumberValidator<float>(MIN_MULTIPLIER, MAX_MULTIPLIER)
      )).OnEntryValueChanged.Subscribe(onPreferenceSaved);
    }

    protected override void onPlayerLoaded(Player _) {
      TimeManagerClean();
    }

    private void onPreferenceSaved(float _1, float _2) {
      TimeManagerClean();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(TimeManager), "Clean")]
    private static void TimeManagerClean() {
      TimeManager.Instance?.SetTimeSpeedMultiplier(1f/modifier!.Value);
    }
  }
}
