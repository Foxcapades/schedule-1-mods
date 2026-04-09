using Fxcpds;
using HarmonyLib;
using MelonLoader;
using MelonLoader.Utils;
using System.Diagnostics;
using System.IO;

#if IL2CPP
using Il2CppScheduleOne.GameTime;
using Il2CppScheduleOne.PlayerScripts;
#elif MONO
using ScheduleOne.GameTime;
using ScheduleOne.PlayerScripts;
#endif

[assembly: MelonID("AdjustableDayLength")]
[assembly: MelonGame("TVGS", "Schedule I")]
[assembly: MelonInfo(typeof(AdjustableDayLength.Mod), AdjustableDayLength.Mod.MOD_NAME, "1.1.2", "Foxcapades")]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.Default)]

namespace AdjustableDayLength {
  public class Mod: FxMod {
    public const string MOD_NAME = "Adjustable Day Length";

    private const float MIN_MULTIPLIER = 0.1f;
    private const float MAX_MULTIPLIER = 60f;

    private static MelonPreferences_Entry<float>? modifier;

    public override void OnInitializeMelon() {
      var preferences = MelonPreferences.CreateCategory(MOD_NAME, MOD_NAME);

      modifier = preferences.CreateEntry(
        identifier:   "modifier",
        display_name: "Day Length Multiplier",
        description:   "Valid range: 0.1 = 10x faster days - 60 = real world time",
        default_value: 1f,
        validator:     new NumberValidator<float>(MIN_MULTIPLIER, MAX_MULTIPLIER)
      );

      modifier!.OnEntryValueChanged.Subscribe(onPreferenceSaved);

      if (getOldValue(out float value)) {
        LoggerInstance.Msg("copying day length multiplier {0} from an older mod version config", value);
        modifier.Value = value;
      }

      base.OnInitializeMelon();
    }

    protected override void onPlayerLoaded(Player _) {
      TimeManagerClean();
    }

    private static void onPreferenceSaved(float _1, float _2) {
      TimeManagerClean();
    }

    private bool getOldValue(out float value) {
      value = 0f;

      if (getOldValue("DayLengthModifier", out float v1)) {
        value = v1;
      }

      if (getOldValue("Day Length Modifier", out float v2)) {
        value = v2;
      }

      return value > 0f;
    }

    private bool getOldValue(string category, out float value) {
      var cat = MelonPreferences.CreateCategory(category);
      var entry = cat.CreateEntry("modifier", value = 0f);

      if (entry.Value > 0f) {

        #if !RELEASE
        Instance.LoggerInstance.Debug("copying modifier {0} from DayLengthModifier", entry.Value);
        #endif

        value = entry.Value;
      }

      cat.DeleteEntry("modifier");
      MelonPreferences.RemoveCategoryFromFile(ConfigPath, cat.Identifier);

      return value > 0;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(TimeManager), "Clean")]
    private static void TimeManagerClean() {
      #if !RELEASE
      Instance.LoggerInstance.Debug("attempting to set multiplier to {0}", 1f/modifier!.Value);
      #endif
      TimeManager.Instance?.SetTimeSpeedMultiplier(1f/modifier!.Value);
    }
  }
}
