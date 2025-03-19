using System;

using V2.Fx;
using V2.Fx.Runtime.CompilerServices;

public static class MemoryCopiedToCallingFrame
{
    public static Wrapper<int> Method(Span<byte> memory)
    {
        var list = new Wrapper<int>(BetterReadOnlySpan.FromInstance(42));
        Unsafe.Copy(memory, in list);
        return list;
    }

    public readonly ref struct Wrapper<T>
    {
        private readonly BetterReadOnlySpan<T> span;

        public Wrapper(BetterReadOnlySpan<T> span)
        {
            this.span = span;
        }
    }
}
