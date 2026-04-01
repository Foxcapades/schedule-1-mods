using MelonLoader;
using MelonLoader.Utils;

#if IL2CPP
using Il2CppInterop.Runtime;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppSystem;
using Il2CppSystem.IO;
#elif MONO
using ScheduleOne.PlayerScripts;
using System.IO;
#endif

#nullable enable
namespace Fxcpds {
  public abstract class FxMod: MelonMod {
    protected const string SCENE_NAME_MAIN = "Main";

    private static FxMod? instance;
    public static FxMod Instance => instance!;

    /// <summary>
    /// The current Unity scene name.
    /// </summary>
    protected string Scene { get ; private set; } = "";

    /// <summary>
    /// Whether the Main Unity scene is currently loaded.
    /// </summary>
    public bool InMainScene { get; private set; }

    /// <summary>
    /// Path to the mod-specific configuration file relative to the UserData
    /// directory.  If the mod does not have a configuration file, this value
    /// should be null.
    /// </summary>
    protected virtual string? configPath { get; }

    /// <summary>
    /// Expanded path to the mod specific configuration file.  If the mod does
    /// not have a configuration file, this value will be null.
    /// </summary>
    public string? ConfigPath =>
      configPath == null ? null : Path.Combine(MelonEnvironment.UserDataDirectory, configPath);

    public override void OnEarlyInitializeMelon() {
      instance = this;
    }

    public override void OnInitializeMelon() {
      #if IL2CPP
      Player.onPlayerSpawned += DelegateSupport.ConvertDelegate<Action<Player>>(onPlayerSpawned);
      #elif MONO
      Player.onPlayerSpawned += onPlayerSpawned;
      #endif
    }

    public override void OnSceneWasLoaded(int _, string sceneName) {
      Scene = sceneName;
      if (sceneName == SCENE_NAME_MAIN) {
        InMainScene = true;
        onMainLoaded();
      }
    }

    public override void OnSceneWasInitialized(int _, string sceneName) {
      if (sceneName == SCENE_NAME_MAIN)
        onMainInitialized();
    }

    public override void OnSceneWasUnloaded(int _, string sceneName) {
      if (Scene == sceneName)
        Scene = "";

      if (sceneName == SCENE_NAME_MAIN) {
        InMainScene = false;
        onMainUnloaded();
      }
    }

    public sealed override void OnPreferencesSaved(string filepath) {
      if (configPath != null && filepath.EndsWith(configPath)) {
        LoggerInstance.Debug("calling onModPreferencesSaved()");
        onModPreferencesSaved();
      }
    }

    public override void OnPreferencesLoaded(string filepath) {
      if (configPath != null && filepath.EndsWith(configPath)) {
        LoggerInstance.Debug("calling onModPreferencesLoaded()");
        onModPreferencesSaved();
      }
    }

    protected virtual void onModPreferencesSaved() {}
    protected virtual void onModPreferencesLoaded() {}

    private void onPlayerSpawned(Player player) {
      if (player.IsLocalPlayer) {
        LoggerInstance.Debug("local player loaded");
        onLocalPlayerLoaded(player);
      }
      onPlayerLoaded(player);
    }

    protected virtual void onPlayerLoaded(Player player) { }

    protected virtual void onLocalPlayerLoaded(Player player) { }

    /// <summary>
    /// Called when the Unity scene "Main" is loaded.
    /// </summary>
    protected virtual void onMainLoaded() { }
    protected virtual void onMainInitialized() { }
    protected virtual void onMainUnloaded() { }
  }
}