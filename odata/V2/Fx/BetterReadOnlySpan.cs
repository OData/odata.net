namespace V2.Fx
{
    using System;
    using System.Runtime.CompilerServices;

    public static class BetterReadOnlySpan
    {
        public static BetterReadOnlySpan<byte> FromMemory(ReadOnlySpan<byte> memory)
        {
            return new BetterReadOnlySpan<byte>(memory, memory.Length);
        }

        public static BetterReadOnlySpan<T> FromMemory<T>(ReadOnlySpan<byte> memory, int length) where T : allows ref struct //// TODO can you use betterspan instead of span for `memory`?
        {
            //// TODO can this be an extension called `cast` or something instead?
            return new BetterReadOnlySpan<T>(memory, length);
        }

        public static unsafe BetterReadOnlySpan<T> FromInstance<T>(scoped in T value) where T : allows ref struct //// TODO is `scoped` ok here? //// TODO is there a way to do this without `in`? //// TODO removing either `scoped` or `in` might cause a test to fail, so you should have a memory integrity issue in mind that removing them fixes
        {
            fixed (T* pointer = &value)
            {
                var span = new ReadOnlySpan<byte>(pointer, Unsafe.SizeOf<T>());

                return BetterReadOnlySpan.FromMemory<T>(span, 1);
            }
        }
    }
}
