using Fxcpds;
using MelonLoader;
#if IL2CPP
using Il2CppScheduleOne;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Core.Items.Framework;
#elif MONO
using ScheduleOne;
using ScheduleOne.DevUtilities;
using ScheduleOne.Core.Items.Framework;
#endif

[assembly: MelonInfo(typeof(GunControl.Mod), GunControl.Mod.MOD_NAME, "1.1.0", "Foxcapades")]
[assembly: MelonGame("TVGS", "Schedule I")]

#nullable enable
namespace GunControl {
  public class Mod: FxMod {
    public const string MOD_NAME = "Gun Control";

    private const string M1911       = "m1911";
    private const string PumpShotgun = "pumpshotgun";
    private const string Revolver    = "revolver";

    private const string M1911Magazine    = "m1911mag";
    private const string RevolverCylinder = "revolvercylinder";
    private const string ShotgunShell     = "shotgunshell";

    protected override void onMainLoaded() {
      var items = Singleton<Registry>.Instance.GetAllItems();

      foreach (var item in items) {
        if (item.Category != EItemCategory.Tools)
          continue;

        switch (item.ID) {
          case M1911:
          case Revolver:
          case PumpShotgun:
            item.legalStatus = ELegalStatus.HighSeverityDrug;
            break;
          case M1911Magazine:
          case RevolverCylinder:
          case ShotgunShell:
            item.legalStatus = ELegalStatus.ModerateSeverityDrug;
            break;
        }
      }
    }
  }
}
