using CartelInfluenceTweaks.state;
using Fxcpds;
using MelonLoader;

#if IL2CPP
using Il2Cpp;
using Il2CppFishNet;
using Il2CppScheduleOne.Cartel;
using Il2CppScheduleOne.Map;
#elif MONO
using FishNet;
using ScheduleOne.Cartel;
using ScheduleOne.Map;
#endif

[assembly: MelonInfo(typeof(CartelInfluenceTweaks.Mod), CartelInfluenceTweaks.Mod.MOD_NAME, "1.0.0", "Foxcapades")]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace CartelInfluenceTweaks {

  public class Mod: FxMod {
    public const string MOD_NAME = "Cartel Influence Tweaks";

    private static readonly ActionStack Actions = new ActionStack();

    internal static Preferences preferences;

    protected override string configPath => "CartelInfluenceTweaks.cfg";

    public override void OnInitializeMelon() {
      preferences.init(this); ;
    }

    internal static void pushState(EMapRegion region, InfluenceAction action) {
      #if !RELEASE
      Instance.LoggerInstance.Debug("Mod.pushState(region: {0}, state: {1})", region, action);
      #endif
      Actions.push(region, action);
    }

    internal static (InfluenceAction, bool) popState(EMapRegion region) {
      #if !RELEASE
      Instance.LoggerInstance.Debug("Mod.popState(region: {0})", region);
      #endif
      return Actions.remove(region);
    }

    internal static bool shouldListen() =>
      InstanceFinder.IsServer && Cartel.Instance.Status == ECartelStatus.Hostile;

    internal static void commonPostfix(EMapRegion region) {
      if (!shouldListen())
        return;

      var (state, found) = popState(region);

      if (found) {
        Instance.LoggerInstance.Error(
          "state ({0}, {1}) was not removed by CartelInfluence.ChangeInfluence",
          region,
          state
        );
      #if !RELEASE
      } else {
        Instance.LoggerInstance.Debug(
          "state for {0} was removed by CartelInfluence.ChangeInfluence",
          region
        );
      #endif
      }
    }
  }
}
