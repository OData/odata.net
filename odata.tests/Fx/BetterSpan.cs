using System.Runtime.CompilerServices;

namespace Fx
{
    public static class BetterSpan
    {
        public static unsafe BetterSpan<T> FromSpan<T>(Span<T> values)
        {
            fixed (T* pointer = values)
            {
                var span = new Span<byte>(pointer, values.Length * Unsafe.SizeOf<T>());

                return new BetterSpan<T>(span, values.Length);
            }
        }

        public static BetterSpan<T> FromMemory<T>(Span<byte> memory, int length) where T : allows ref struct
        {
            return new BetterSpan<T>(memory, length);
        }

        public static unsafe BetterSpan<T> FromInstance<T>(scoped in T value) where T : allows ref struct
        {
            fixed (T* pointer = &value)
            {
                var span = new Span<byte>(pointer, Unsafe.SizeOf<T>());

                return BetterSpan.FromMemory<T>(span, 1);
            }
        }

        public static unsafe BetterSpan<T> FromInstance2<T>(in T value) where T : allows ref struct
        {
            fixed (T* pointer = &value)
            {
                var span = new Span<byte>(pointer, Unsafe.SizeOf<T>());

                return BetterSpan.FromMemory<T>(span, 1);
            }
        }

        public static BetterSpan2<T> FromMemory2<T>(Span<byte> memory, int length) where T : allows ref struct
        {
            return new BetterSpan2<T>(memory, length);
        }

        public static BetterSpan2<T> FromInstance3<T>(T value) where T : allows ref struct
        {
            return new BetterSpan2<T>(value);
        }
    }
}
