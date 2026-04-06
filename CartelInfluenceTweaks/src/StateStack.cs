#if IL2CPP
using Il2CppScheduleOne.Map;
#elif MONO
using ScheduleOne.Map;
#endif

namespace CartelInfluenceTweaks {
  internal class StateStack {
    private StackNode? first;

    public void push(EMapRegion region, State state) {
      first = new StackNode(region, state, first);
    }

    public (State, bool) remove(EMapRegion region) {
      StackNode? prev = null;
      StackNode? next = first;

      while (next != null) {
        if (next.region == region) {
          if (prev != null)
            prev.next = next.next;
          else
            first = next.next;

          return (next.state, true);
        }

        prev = next;
        next = next.next;
      }

      return (default, false);
    }
  }

  internal class StackNode {
    internal readonly EMapRegion region;
    internal readonly State state;
    internal StackNode? next;

    public StackNode(EMapRegion region, State state, StackNode? next) {
      this.region = region;
      this.state = state;
      this.next = next;
    }
  }
}
