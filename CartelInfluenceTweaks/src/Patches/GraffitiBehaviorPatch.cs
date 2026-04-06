using Fxcpds;
using HarmonyLib;
using System.Runtime.CompilerServices;

#if IL2CPP
using Il2CppScheduleOne.Cartel;
using Il2CppScheduleOne.Graffiti;
using Il2CppScheduleOne.NPCs.Behaviour;
#elif MONO
using ScheduleOne.Cartel;
using ScheduleOne.Graffiti;
using ScheduleOne.NPCs.Behaviour;
using System.Reflection;
#endif

namespace CartelInfluenceTweaks.Patches {

  [HarmonyPatch(typeof(GraffitiBehaviour), nameof(GraffitiBehaviour.Disable))]
  public static class GraffitiBehaviorPatch {

    #if IL2CPP

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static WorldSpraySurface getSurface(this GraffitiBehaviour parent) =>
      parent._spraySurface!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool isCompleted(this GraffitiBehaviour parent) =>
      parent._graffitiCompleted;

    #elif MONO

    private static FieldInfo? surfaceField;
    private static FieldInfo? completedField;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static WorldSpraySurface getSurface(this GraffitiBehaviour parent) =>
      (WorldSpraySurface) (surfaceField == null
        ? surfaceField = typeof(GraffitiBehaviour).GetField("_spraySurface", BindingFlags.NonPublic | BindingFlags.Instance)
        : surfaceField)!.GetValue(parent);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool isCompleted(this GraffitiBehaviour parent) =>
      (bool) (completedField == null
        ? completedField = typeof(GraffitiBehaviour).GetField("_graffitiCompleted", BindingFlags.NonPublic | BindingFlags.Instance)
        : completedField)!.GetValue(parent);

    #endif

    /// <param name="__state">Whether cartel influence should be
    /// awarded.</param>
    // ReSharper disable once UnusedMember.Local InvalidXmlDocComment
    static bool Prefix(GraffitiBehaviour __instance, out bool __state) {
      __state = false;

      if (!__instance.Enabled)
        return false;

      var region = __instance.getSurface().Region;

      #if !RELEASE
      FxMod.Instance.LoggerInstance.Debug("GraffitiBehavior.Disable.Prefix({0})", region);
      #endif

      if (Mod.shouldListen()) {
        if (__instance.isCompleted()) {
          __state = true;
          Mod.pushState(region, State.CartelGraffiti);
        } else {
          Mod.pushState(region, State.GraffitiInterrupted);
        }
      }

      return true;
    }

    /// <param name="__state">Whether cartel influence should be
    /// awarded.</param>
    // ReSharper disable once UnusedMember.Local InvalidXmlDocComment
    static void Postfix(GraffitiBehaviour __instance, bool __state) {
      if (!__state)
        return;

      var region = __instance.getSurface().Region;

      #if !RELEASE
      FxMod.Instance.LoggerInstance.Debug("GraffitiBehavior.Disable.Postfix({0})", region);
      #endif

      Cartel.Instance.Influence.ChangeInfluence(region, 1f);

      Mod.commonPostfix(region);
    }
  }
}