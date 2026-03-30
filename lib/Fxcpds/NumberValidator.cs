using System;
using MelonLoader.Preferences;

namespace Fxcpds {
  public sealed class NumberValidator<T>: ValueValidator where T: IComparable<T> {
    private readonly T min;
    private readonly T max;

    public NumberValidator(T min, T max) {
      this.min = min;
      this.max = max;
    }

    public override bool IsValid(object value) {
      var typed = (T) value;
      return typed.CompareTo(min) > -1 && typed.CompareTo(max) < 1;
    }

    public override object EnsureValid(object value) {
      var typed = (T) value;

      if (typed.CompareTo(min) < 0)
        return min;

      if (typed.CompareTo(max) > 0)
        return max;

      return value;
    }
  }
}