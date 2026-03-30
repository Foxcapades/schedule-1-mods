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
  public class Mod: FxMod<Mod> {
    public const string MOD_NAME = "Gun Control";

    protected override void onMainLoaded() {
      var items = Singleton<Registry>.Instance.GetAllItems();

      foreach (var item in items) {
        if (item.Category != EItemCategory.Tools)
          continue;

        switch (item.ID) {
          case Item.M1911:
          case Item.Revolver:
          case Item.PumpShotgun:
            item.legalStatus = ELegalStatus.HighSeverityDrug;
            break;
          case Item.M1911Magazine:
          case Item.RevolverCylinder:
          case Item.ShotgunShell:
            item.legalStatus = ELegalStatus.ModerateSeverityDrug;
            break;
        }
      }
    }
  }
}
