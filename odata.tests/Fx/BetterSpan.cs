using System.Runtime.CompilerServices;

namespace Fx
{
    public static class BetterSpan
    {
        public static unsafe BetterSpan<T> FromSpan<T>(Span<T> values)
        {
            var span = new Span<byte>(&values, values.Length * Unsafe.SizeOf<T>());

            return new BetterSpan<T>(span, values.Length);
        }

        public static BetterSpan<T> FromMemory<T>(Span<byte> memory, int length) where T : allows ref struct
        {
            return new BetterSpan<T>(memory, length);
        }

        public static unsafe BetterSpan<T> FromInstance<T>(T value) where T : allows ref struct
        {
            var span = new Span<byte>(&value, Unsafe.SizeOf<T>());

            return BetterSpan.FromMemory<T>(span, 1);
        }

        public static unsafe BetterSpan<T> FromInstance2<T>(in T value, ref Span<byte> destination) where T : allows ref struct
        {
            fixed (T* pointer = &value)
            {
                destination = new Span<byte>(pointer, Unsafe.SizeOf<T>());
            }

            return BetterSpan.FromMemory<T>(destination, 1);
        }
    }
}
