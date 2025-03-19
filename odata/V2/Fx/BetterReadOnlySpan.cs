namespace V2.Fx
{
    using System;
    using System.Runtime.CompilerServices;

    public static class BetterReadOnlySpan
    {
        public static BetterReadOnlySpan<T> FromMemory<T>(Span<byte> memory, int length) where T : allows ref struct //// TODO can you use betterspan instead of span for `memory`?
        {
            //// TODO can this be an extension called `cast` or something instead?
            return new BetterReadOnlySpan<T>(memory, length);
        }

        public static unsafe BetterReadOnlySpan<T> FromInstance<T>(in T value) where T : allows ref struct //// TODO `value` is `scoped` in the old code... //// TODO is there a way to do this without `in`?
        {
            fixed (T* pointer = &value)
            {
                var span = new Span<byte>(pointer, Unsafe.SizeOf<T>());

                return BetterReadOnlySpan.FromMemory<T>(span, 1);
            }
        }
    }
}
