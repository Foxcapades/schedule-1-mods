using MelonLoader;

namespace Fxcpds {
  internal static class DebugTools {
    public static void Debug(this MelonLogger.Instance logger, string message) {
      #if !RELEASE
      logger.Msg("[DEBUG] {0}", message);
      #endif
    }

    public static void Debug(this MelonLogger.Instance logger, string message, object? a1) {
      #if !RELEASE
      logger.Msg("[DEBUG] {0}", string.Format(message, a1));
      #endif
    }

    public static void Debug(this MelonLogger.Instance logger, string message, object? a1, object? a2) {
      #if !RELEASE
      logger.Msg("[DEBUG] {0}", string.Format(message, a1, a2));
      #endif
    }

    public static void Debug(this MelonLogger.Instance logger, string message, object? a1, object? a2, object? a3) {
      #if !RELEASE
      logger.Msg("[DEBUG] {0}", string.Format(message, a1, a2, a3));
      #endif
    }

    public static void Debug(this MelonLogger.Instance logger, string message, object? a1, object? a2, object? a3, object? a4) {
      #if !RELEASE
      logger.Msg("[DEBUG] {0}", string.Format(message, a1, a2, a3, a4));
      #endif
    }
  }
}