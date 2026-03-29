using MelonLoader;
#if IL2CPP
using Il2CppInterop.Runtime;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppSystem;
#elif MONO
using ScheduleOne.PlayerScripts;
#endif

#nullable enable
namespace Fxcpds {
  public class FxMod: MelonMod {
    protected static string Scene { get ; private set; } = Fxcpds.Scene.Undefined;

    public static MelonLogger.Instance Logger => Melon<FxMod>.Logger;

    protected static bool InMainScene { get; private set; }

    public string? ConfigPath { get; protected set; }

    public override void OnInitializeMelon() {
      #if IL2CPP
      Player.onPlayerSpawned += DelegateSupport.ConvertDelegate<Action<Player>>(onPlayerSpawned);
      #elif MONO
      Player.onPlayerSpawned += onPlayerSpawned;
      #endif
    }

    public override void OnSceneWasLoaded(int _, string sceneName) {
      Scene = sceneName;
      if (sceneName == Fxcpds.Scene.Main) {
        InMainScene = true;
        onMainLoaded();
      }
    }

    public override void OnSceneWasInitialized(int _, string sceneName) {
      if (sceneName == Fxcpds.Scene.Main)
        onMainInitialized();
    }

    public override void OnSceneWasUnloaded(int _, string sceneName) {
      if (Scene == sceneName)
        Scene = Fxcpds.Scene.Undefined;

      if (sceneName == Fxcpds.Scene.Main) {
        InMainScene = false;
        onMainUnloaded();
      }
    }

    private void onPlayerSpawned(Player player) {
      if (player.IsLocalPlayer) {
        Logger.Debug("local player loaded");
        onLocalPlayerLoaded(player);
      }
    }

    protected virtual void onLocalPlayerLoaded(Player player) { }

    protected virtual void onMainLoaded() { }
    protected virtual void onMainInitialized() { }
    protected virtual void onMainUnloaded() { }
  }
}