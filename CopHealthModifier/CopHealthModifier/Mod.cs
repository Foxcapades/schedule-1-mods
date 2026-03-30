using Fxcpds;
using MelonLoader;
#if IL2CPP
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.Police;
#elif MONO
using ScheduleOne.PlayerScripts;
using ScheduleOne.Police;
#endif

[assembly: MelonInfo(typeof(CopHealthModifier.Mod), CopHealthModifier.Mod.MOD_NAME, "1.1.0", "Foxcapades")]
[assembly: MelonGame("TVGS", "Schedule I")]

#nullable enable
namespace CopHealthModifier {
  public class Mod: FxMod {
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
        validator: new NumberValidator<float>(0.1f, float.MaxValue)
      );
    }

    protected override void onLocalPlayerLoaded(Player player) {
      applyModifier();
    }

    public override void OnPreferencesSaved() {
      applyModifier();
    }

    private void applyModifier() {
      if (Player.Local == null)
        return;

      var officers = PoliceOfficer.Officers;

      if (officers.Count == 0) {
        LoggerInstance.Error("No police officers found to modify!");
        return;
      }

      var mult = multiplier!.Value;
      var newHealth = defaultCopHealth * mult;

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
