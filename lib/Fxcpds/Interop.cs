#if IL2CPP
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.Runtime;
#elif MONO
using Object = UnityEngine.Object;
#endif

#nullable enable
namespace Fxcpds {
  public static class Interop {
    #if IL2CPP
    public static T? cast<T>(Il2CppObjectBase? obj) where T: Il2CppObjectBase {
      return obj == null ? null : Il2CppObjectPool.Get<T>(obj.Pointer);
    }
    #elif MONO
    public static T? cast<T>(Object? obj) where T: Object {
      return obj == null ? null : (T) obj;
    }
    #endif
  }
}