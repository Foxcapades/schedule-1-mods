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

[assembly: MelonInfo(typeof(IllegalSeeds.Mod), IllegalSeeds.Mod.MOD_NAME, "1.1.0", "Foxcapades")]
[assembly: MelonGame("TVGS", "Schedule I")]

#nullable enable
namespace IllegalSeeds {
  public class Mod: FxMod {
    public const string MOD_NAME = "Illegal Seeds";

    private const string CocaSeed             = "cocaseed";
    private const string GranddaddyPurpleSeed = "granddaddypurpleseed";
    private const string GreenCrackSeed       = "greencrackseed";
    private const string OGKushSeed           = "ogkushseed";
    private const string SourDieselSeed       = "sourdieselseed";

    protected override void onMainLoaded() {
      foreach (var item in Singleton<Registry>.Instance.GetAllItems()) {
        if (item.Category != EItemCategory.Agriculture)
          continue;

        switch (item.ID) {
          case CocaSeed:
            item.legalStatus = ELegalStatus.HighSeverityDrug;
            break;

          case GranddaddyPurpleSeed:
          case GreenCrackSeed:
          case OGKushSeed:
          case SourDieselSeed:
            item.legalStatus = ELegalStatus.ModerateSeverityDrug;
            break;
        }
      }
    }
  }
}
