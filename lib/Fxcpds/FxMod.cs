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

    private static FxMod? instance;
    public static FxMod Instance => instance!;

    public static MelonLogger.Instance Logger => instance!.LoggerInstance;

    #if USE_ON_MAIN || USE_SCENES
    protected const string SCENE_NAME_MAIN = "Main";

    /// <summary>
    /// The current Unity scene name.
    /// </summary>
    protected string Scene { get ; private set; } = "";

    /// <summary>
    /// Whether the Main Unity scene is currently loaded.
    /// </summary>
    public bool InMainScene { get; private set; }
    #endif

    #if USE_CONFIG_FILE && !USE_CONFIG

    /// <summary>
    /// Path to the mod-specific configuration file relative to the UserData
    /// directory.
    /// </summary>
    protected virtual string configPath { get; }

    #endif

    #if USE_CONFIG && !USE_CONFIG_FILE

    /// <summary>
    /// Path to the MelonLoader default configuration file.
    /// </summary>
    protected string configPath => "MelonPreferences.cfg";

    #endif

    #if USE_CONFIG || USE_CONFIG_FILE

    /// <summary>
    /// Expanded path to the mod configuration file.
    /// </summary>
    public string ConfigPath =>
      Path.Combine(MelonEnvironment.UserDataDirectory, configPath);

    #endif

    public override void OnEarlyInitializeMelon() {
      instance = this;
    }

    #if USE_ON_PLAYER || USE_ON_PLAYER_LOCAL

    public override void OnInitializeMelon() {
      #if IL2CPP
      Player.onPlayerSpawned += DelegateSupport.ConvertDelegate<Action<Player>>(onPlayerSpawned);
      #elif MONO
      Player.onPlayerSpawned += onPlayerSpawned;
      #endif
    }

    #endif

    #if  USE_ON_MAIN || USE_SCENES

    public override void OnSceneWasLoaded(int _, string sceneName) {
      Scene = sceneName;
      if (sceneName == SCENE_NAME_MAIN) {
        InMainScene = true;

        #if USE_ON_MAIN
        onMainLoaded();
        #endif
      }
    }

    #endif

    #if USE_ON_MAIN

    public override void OnSceneWasInitialized(int _, string sceneName) {
      if (sceneName == SCENE_NAME_MAIN)
        onMainInitialized();
    }

    #endif

    #if USE_ON_MAIN || USE_SCENES
    public override void OnSceneWasUnloaded(int _, string sceneName) {
      if (Scene == sceneName)
        Scene = "";

      if (sceneName == SCENE_NAME_MAIN) {
        InMainScene = false;

        #if USE_ON_MAIN
        onMainUnloaded();
        #endif
      }
    }
    #endif

    #if USE_CONFIG_FILE || USE_CONFIG

    public sealed override void OnPreferencesSaved(string filepath) {
      if (filepath.EndsWith(configPath)) {
        #if !RELEASE
        Logger.Debug("calling onModPreferencesSaved");
        #endif
        onModPreferencesSaved();
      }
    }

    protected virtual void onModPreferencesSaved() {}

    #endif

    #if USE_ON_PLAYER || USE_ON_PLAYER_LOCAL

    private void onPlayerSpawned(Player player) {
      #if USE_ON_PLAYER_LOCAL
      #if !RELEASE
      Logger.Debug("calling onLocalPlayerLoaded");
      #endif
      if (player.IsLocalPlayer) {
        onLocalPlayerLoaded(player);
      }
      #endif
      #if USE_ON_PLAYER
      #if !RELEASE
      Logger.Debug("calling onPlayerLoaded");
      #endif
      onPlayerLoaded(player);
      #endif
    }

    #endif

    #if USE_ON_PLAYER
    protected virtual void onPlayerLoaded(Player player) { }
    #endif

    #if USE_ON_PLAYER_LOCAL
    protected virtual void onLocalPlayerLoaded(Player player) { }
    #endif

    #if USE_ON_MAIN
    /// <summary>
    /// Called when the Unity scene "Main" is loaded.
    /// </summary>
    protected virtual void onMainLoaded() { }
    protected virtual void onMainInitialized() { }
    protected virtual void onMainUnloaded() { }
    #endif
  }
}