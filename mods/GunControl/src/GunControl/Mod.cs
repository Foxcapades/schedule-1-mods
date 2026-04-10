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

[assembly: MelonID("GunControl")]
[assembly: MelonInfo(typeof(GunControl.Mod), GunControl.Mod.MOD_NAME, "1.1.0", "Foxcapades")]
[assembly: MelonGame("TVGS", "Schedule I")]

#nullable enable
namespace GunControl {
  public class Mod: FxMod {
    public const string MOD_NAME = "Gun Control";

    protected override void onMainLoaded() {
      var items = Singleton<Registry>.Instance.GetAllItems();

      foreach (var item in items) {
        if (item.Category != EItemCategory.Tools)
          continue;

        switch (item.ID) {
          case "m1911":
          case "revolver":
          case "pumpshotgun":
            item.legalStatus = ELegalStatus.HighSeverityDrug;
            break;
          case "m1911mag":
          case "revolvercylinder":
          case "shotgunshell":
            item.legalStatus = ELegalStatus.ModerateSeverityDrug;
            break;
        }
      }
    }
  }
}
