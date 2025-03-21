using V2.Fx;

using Unsafe = System.Runtime.CompilerServices.Unsafe;
using Unsafe2 = V2.Fx.Runtime.CompilerServices.Unsafe;

public static class CopiedMemoryLeavingFrame
{
    public static BetterReadOnlySpan<Wrapper<int>> Method()
    {
        var list = new Wrapper<int>(BetterReadOnlySpan.FromInstance(42));
        ByteSpan memory = stackalloc byte[Unsafe.SizeOf<Wrapper<int>>()];
        Unsafe2.Copy(memory, in list);

        var nextValue = BetterReadOnlySpan.FromInstance(67);
        var previousNode = BetterReadOnlySpan.FromMemory<Wrapper<int>>(memory, 1);

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