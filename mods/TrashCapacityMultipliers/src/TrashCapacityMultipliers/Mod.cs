using Fxcpds;
using HarmonyLib;
using MelonLoader;
using UnityEngine;
using System.Diagnostics;
using System.Runtime.CompilerServices;
#if IL2CPP
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.ObjectScripts.WateringCan;
using Il2CppScheduleOne.Property;
using Il2CppSystem;

#elif MONO
using ScheduleOne.ItemFramework;
using ScheduleOne.ObjectScripts;
using ScheduleOne.ObjectScripts.WateringCan;
using ScheduleOne.Property;
using System;
#endif

[assembly: Debuggable(DebuggableAttribute.DebuggingModes.Default)]
[assembly: MelonInfo(typeof(TrashCapacity.Mod), TrashCapacity.Mod.MOD_NAME, "1.1.1", "Foxcapades")]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace TrashCapacity {
  public class Mod: FxMod {
    public const string MOD_NAME = "Trash Capacity";

    private static MelonPreferences_Entry<float>? binMultiplier;
    private static MelonPreferences_Entry<float>? grabberMultiplier;

    private static float initialBinCapacity;

    public override void OnInitializeMelon() {
      var category = MelonPreferences.CreateCategory(MOD_NAME);
      var validator = new NumberValidator<float>(1f, 100f);

      (binMultiplier = category.CreateEntry(
        identifier: "trashCanCapacityMultiplier",
        default_value: 2f,
        display_name: "Bin Capacity Multiplier",
        oldIdentifier: "capacityMultiplier",
        validator: validator
      )).OnEntryValueChanged.Subscribe(onBinPreferencesSaved);

      grabberMultiplier = category.CreateEntry(
        identifier: "trashGrabberCapacityMultiplier",
        default_value: 1f,
        display_name: "Grabber Capacity Multiplier",
        validator: validator
      );
    }

    private void onBinPreferencesSaved(float o, float n) {
      if (!Mathf.Approximately(n, o))
        updateBins();
    }

    private void updateBins() {
      if (!InMainScene || initialBinCapacity == 0f)
        return;

      foreach (var property in Property.Properties) {
        foreach (var trashcan in property.GetBuildablesOfType<TrashContainerItem>()) {
          trashcan.Container.TrashCapacity = calcCapacity();
        }
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int calcCapacity() =>
      (int) Math.Round(initialBinCapacity * binMultiplier!.Value, MidpointRounding.AwayFromZero);

    [HarmonyPatch(typeof(TrashGrabberInstance), nameof(TrashGrabberInstance.GetTotalSize))]
    private static class GrabberPatch {
      // ReSharper disable once UnusedMember.Local
      static int Postfix(int result) {
        var value = result / grabberMultiplier!.Value;

        return value > 0f && value < 1f
          ? 1
          : Mathf.FloorToInt(value);
      }
    }

    [HarmonyPatch(typeof(TrashContainerItem), nameof(TrashContainerItem.InitializeGridItem))]
    private static class BinPatch {
      // ReSharper disable once UnusedMember.Local
      static void Prefix(ItemInstance instance, TrashContainerItem __instance) {
        if (instance.ID == "trashcan") {
          if (initialBinCapacity == 0f)
            initialBinCapacity = __instance.Container.TrashCapacity;
          __instance.Container.TrashCapacity = calcCapacity();
        }
      }
    }
  }
}