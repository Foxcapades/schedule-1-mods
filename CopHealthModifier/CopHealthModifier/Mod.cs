using MelonLoader;
using MelonLoader.Preferences;
#if IL2CPP
using Il2CppInterop.Runtime;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.Police;
using Il2CppSystem;
#elif MONO
using ScheduleOne.Police;
using ScheduleOne.PlayerScripts;
#endif

[assembly: MelonInfo(typeof(CopHealthModifier.Mod), CopHealthModifier.Mod.MOD_NAME, "1.1.0", "Foxcapades")]
[assembly: MelonGame("TVGS", "Schedule I")]

#nullable enable
namespace CopHealthModifier {
  public class Mod: MelonMod {
    public const string MOD_NAME = "Cop Health Modifier";
    private const string MOD_PREF_NAME = "hpMultiplier";

    private static MelonPreferences_Entry<float>? multiplier;

    private static float defaultCopHealth;

    public override void OnInitializeMelon() {
      var preferences = MelonPreferences.CreateCategory(MOD_NAME, MOD_NAME);
      multiplier = preferences.CreateEntry(
        identifier: MOD_PREF_NAME,
        default_value: 1f,
        display_name: "Cop Health Multiplier",
        validator: new Validator()
      );
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName) {
      if (sceneName != "Main")
        return;

#if IL2CPP
      Player.onPlayerSpawned += DelegateSupport.ConvertDelegate<Action<Player>>(onPlayerSpawn);
#elif MONO
      Player.onPlayerSpawned += onPlayerSpawn;
#endif
    }

    public override void OnPreferencesSaved() {
      applyModifier();
    }

    private sealed class Validator: ValueValidator {
      public override bool IsValid(object value) {
        var fValue = (float)value;
        return fValue > 0.1f;
      }

      public override object EnsureValid(object value) {
        var fValue = (float)value;
        return fValue < 0.1f ? 0.1f : fValue;
      }
    }

    private static void onPlayerSpawn(Player player) {
      applyModifier();
    }

    private static void applyModifier() {
      if (Player.Local == null)
        return;

      var officers = PoliceOfficer.Officers;

      if (officers.Count == 0) {
        Melon<Mod>.Instance.LoggerInstance.Error("No police officers found to modify!");
        return;
      }

      var mult = multiplier!.Value;
      var newHealth = defaultCopHealth * mult;

      Melon<Mod>.Instance.LoggerInstance.Msg($"Multiplying cop health by {mult}");

      foreach (var officer in officers) {
        if (defaultCopHealth == 0) {
          defaultCopHealth = officer.Health.MaxHealth;
          newHealth = defaultCopHealth * mult;
        }

        if ((int)officer.Health.MaxHealth * 100 != (int)newHealth * 100) {
          officer.Health.MaxHealth = newHealth;
          officer.Health.RestoreHealth();
        }
      }
    }
  }
}
