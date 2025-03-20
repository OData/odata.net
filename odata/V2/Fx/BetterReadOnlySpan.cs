namespace V2.Fx
{
    using System;
    using System.Runtime.CompilerServices;

    public static class BetterReadOnlySpan
    {
        public static BetterReadOnlySpan<byte> FromMemory(DifferentMemory memory)
        {
            return BetterReadOnlySpan<byte>.Create(memory);
        }

        public static BetterReadOnlySpan<T> FromMemory<T>(DifferentMemory memory, int length) where T : allows ref struct
        {
            //// TODO can this be an extension called `cast` or something instead?
            return BetterReadOnlySpan<T>.Create(memory, length);
        }

        public static unsafe BetterReadOnlySpan<T> FromInstance<T>(scoped in T value) where T : allows ref struct //// TODO is `scoped` ok here? //// TODO is there a way to do this without `in`? //// TODO removing either `scoped` or `in` might cause a test to fail, so you should have a memory integrity issue in mind that removing them fixes
        {
            return BetterReadOnlySpan<T>.Create(value);
        }
    }
}
