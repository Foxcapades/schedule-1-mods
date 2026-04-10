#if IL2CPP
using Il2CppScheduleOne.Map;
#elif MONO
using ScheduleOne.Map;
#endif

namespace CartelInfluenceTweaks.state {

  /// <summary>
  /// Queued influence change states as a singly linked list.
  /// </summary>
  internal class ActionStack {
    private StackNode? first;

    public void push(EMapRegion region, InfluenceAction action) {
      first = new StackNode(region, action, first);
    }

    public (InfluenceAction, bool) remove(EMapRegion region) {
      StackNode? prev = null;
      StackNode? next = first;

      while (next != null) {
        if (next.region == region) {
          if (prev != null)
            prev.next = next.next;
          else
            first = next.next;

          return (next.Action, true);
        }

        prev = next;
        next = next.next;
      }

      return (default, false);
    }
  }

  internal class StackNode {
    internal readonly EMapRegion region;
    internal readonly InfluenceAction Action;
    internal StackNode? next;

    public StackNode(EMapRegion region, InfluenceAction action, StackNode? next) {
      this.region = region;
      this.Action = action;
      this.next = next;
    }
  }
}
