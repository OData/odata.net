using System.Runtime.CompilerServices;

using V2.Fx;

public static class BetterReadOnlySpanFactoryMemoryIntegrityTestsResourcesStackAllocLeavingFrame
{
    private static BetterReadOnlySpan<string> Method()
    {
        ByteSpan span = stackalloc byte[Unsafe.SizeOf<string>()];
        var betterspan = BetterReadOnlySpan.FromMemory<string>(span, 1);

        return betterspan;
    }
}
