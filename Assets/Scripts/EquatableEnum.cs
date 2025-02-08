using System;
using Unity.Collections.LowLevel.Unsafe;

public struct EquatableEnum<TEnum> : IEquatable<EquatableEnum<TEnum>> where TEnum : struct, Enum {
    private int Value;

    public bool Equals(EquatableEnum<TEnum> other) 
        => Value == other.Value;

    public override int GetHashCode() 
        => Value.GetHashCode();

    public static implicit operator TEnum(EquatableEnum<TEnum> value)
        => UnsafeUtility.As<int, TEnum>(ref value.Value);
    public static implicit operator EquatableEnum<TEnum>(TEnum value)
        => new() { Value = UnsafeUtility.EnumToInt(value) };
}
