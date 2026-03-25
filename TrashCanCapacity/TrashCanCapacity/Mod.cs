using HarmonyLib;
using MelonLoader;
using UnityEngine;
#if IL2CPP
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.Property;
using Il2CppSystem;
#elif MONO
using ScheduleOne.ItemFramework;
using ScheduleOne.ObjectScripts;
using ScheduleOne.Property;
using System;
#endif

[assembly: MelonInfo(typeof(TrashCanCapacity.Mod), TrashCanCapacity.Mod.MOD_NAME, "1.0.0", "Foxcapades")]
[assembly: MelonGame("TVGS", "Schedule I")]

#nullable enable
namespace TrashCanCapacity {
  public class Mod: MelonMod {
    public const string MOD_NAME = "TrashCanCapacity";

    private static MelonPreferences_Entry<float> multiplier;
    private static float initialValue;
    private static bool inMain;

    public override void OnInitializeMelon() {
      multiplier = MelonPreferences.CreateEntry(
        "Trash Can Capacity",
        "capacityMultiplier",
        2f,
        "Multiplier"
      );

      multiplier.OnEntryValueChanged.Subscribe(onPreferencesSaved);
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName) {
      if (sceneName == "Main")
        inMain = true;
    }

    public override void OnSceneWasUnloaded(int buildIndex, string sceneName) {
      if (sceneName == "Main")
        inMain = false;
    }

    private static void onPreferencesSaved(float o, float n) {
      if (!Mathf.Approximately(n, o))
        updateExisting();
    }

    private static int calcCapacity() =>
      (int) Math.Round(initialValue * multiplier.Value, MidpointRounding.AwayFromZero);

    private static void updateExisting() {
      if (!inMain || initialValue == 0f)
        return;

      foreach (var property in Property.Properties) {
        foreach (var trashcan in property.GetBuildablesOfType<TrashContainerItem>()) {
          trashcan.Container.TrashCapacity = calcCapacity();
        }
      }
    }

    [HarmonyPatch(typeof(TrashContainerItem), nameof(TrashContainerItem.InitializeGridItem))]
    private static class Patch {
      static void Prefix(ItemInstance instance, TrashContainerItem __instance) {
        if (instance.ID == "trashcan") {
          if (initialValue == 0f)
            initialValue = __instance.Container.TrashCapacity;
          __instance.Container.TrashCapacity = calcCapacity();
        }
      }
    }
  }
}
