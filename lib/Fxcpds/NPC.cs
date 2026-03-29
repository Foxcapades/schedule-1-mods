#if IL2CPP
using GameNPC = Il2CppScheduleOne.NPCs.NPC;

using Il2CppScheduleOne.NPCs;
#elif MONO
using GameNPC = ScheduleOne.NPCs.NPC;

using ScheduleOne.NPCs;
#endif

#nullable enable
namespace Fxcpds {
  public static class NPC {
    public const string Oscar = "oscar_holland";
    public const string Dan   = "dan_samwell";
    public const string Hank  = "hank_stevenson";

    public static T? Get<T>(string id) where T: GameNPC {
      FxMod.Logger.Debug("getting npc ({0})", id);
      var ret = Interop.cast<T>(NPCManager.GetNPC(id));
      FxMod.Logger.Debug("got npc ({0})", ret);
      return ret;
    }

  }
}