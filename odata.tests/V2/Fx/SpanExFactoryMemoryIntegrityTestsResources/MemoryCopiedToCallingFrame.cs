using System;

using V2.Fx;
using V2.Fx.Runtime.InteropServices;

public static class SpanExFactoryMemoryIntegrityTestsResourcesMemoryCopiedToCallingFrame
{
    public static Wrapper<int> Method(Span<byte> memory)
    {
        var value = 42;
        var list = new Wrapper<int>(SpanEx.FromInstance(ref value));
        MemoryMarshal.Write(memory, in list);
        return list;
    }

    public readonly ref struct Wrapper<T>
    {
        private readonly SpanEx<T> span;

        public Wrapper(SpanEx<T> span)
        {
            this.span = span;
        }
    }
}
