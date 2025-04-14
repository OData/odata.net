using System.Runtime.CompilerServices;

using V2.Fx;
using V2.Fx.Runtime.InteropServices;

public static class SpanExFactoryMemoryIntegrityTestsResourcesCopiedMemoryLeavingFrame
{
    public static SpanEx<Wrapper<int>> Method()
    {
        var element = 42;
        var list = new Wrapper<int>(SpanEx.FromInstance(ref element));
        ByteSpan memory = stackalloc byte[Unsafe.SizeOf<Wrapper<int>>()];
        MemoryMarshal.Write(memory, in list);

        var nextElement = 67;
        var nextValue = SpanEx.FromInstance(ref nextElement);
        var previousNode = SpanEx.FromMemory<Wrapper<int>>(memory, 1);

        return previousNode;
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