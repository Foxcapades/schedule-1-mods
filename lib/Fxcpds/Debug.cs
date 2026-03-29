using MelonLoader;

#nullable enable
namespace Fxcpds {
  internal static class DebugTools {
    public static void Debug(this MelonLogger.Instance logger, string message) {
      #if DEBUG
      logger.Msg("[DEBUG] {0}", message);
      #endif
    }

    public static void Debug(this MelonLogger.Instance logger, string message, object? a1) {
      #if DEBUG
      logger.Msg("[DEBUG] {0}", string.Format(message, a1));
      #endif
    }
  }
}