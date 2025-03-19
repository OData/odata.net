namespace V2.Fx
{
    using System;

    public static class BetterReadOnlySpan
    {
        public static BetterReadOnlySpan<T> FromMemory<T>(Span<byte> memory, int length) where T : allows ref struct //// TODO can you use betterspan instead of span for `memory`?
        {
            //// TODO can this be an extension called `cast` or something instead?
            return new BetterReadOnlySpan<T>(memory, length);
        }
    }
}
