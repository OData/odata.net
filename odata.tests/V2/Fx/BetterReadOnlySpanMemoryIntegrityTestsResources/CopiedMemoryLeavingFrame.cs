using System.Runtime.CompilerServices;

using V2.Fx;
using V2.Fx.Runtime.InteropServices;

public static class BetterReadOnlySpanMemoryIntegrityTestsResourcesCopiedMemoryLeavingFrame
{
    public static BetterReadOnlySpan<Wrapper<int>> Method()
    {
        var element = 42;
        var list = new Wrapper<int>(BetterReadOnlySpan.FromInstance(ref element));
        ByteSpan memory = stackalloc byte[Unsafe.SizeOf<Wrapper<int>>()];
        MemoryMarshal.Write(memory, in list);

        var nextElement = 67;
        var nextValue = BetterReadOnlySpan.FromInstance(ref nextElement);
        var previousNode = BetterReadOnlySpan<Wrapper<int>>.Create(memory, 1);

        return previousNode;
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