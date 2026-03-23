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

[assembly: MelonInfo(typeof(IllegalSeeds.Mod), IllegalSeeds.Mod.MOD_NAME, "1.1.0", "Foxcapades")]
[assembly: MelonGame("TVGS", "Schedule I")]

#nullable enable
namespace IllegalSeeds {
  public class Mod: MelonMod {
    public const string MOD_NAME = "Illegal Seeds";

    public override void OnSceneWasLoaded(int buildIndex, string sceneName) {
      if (sceneName != "Main")
        return;

      foreach (var item in Singleton<Registry>.Instance.GetAllItems()) {
        if (item.Category != EItemCategory.Agriculture)
          continue;

        switch (item.ID) {
          case "cocaseed":
            item.legalStatus = ELegalStatus.HighSeverityDrug;
            break;

          case "granddaddypurpleseed":
          case "greencrackseed":
          case "ogkushseed":
          case "sourdieselseed":
            item.legalStatus = ELegalStatus.ModerateSeverityDrug;
            break;
        }
      }
    }
  }
}
