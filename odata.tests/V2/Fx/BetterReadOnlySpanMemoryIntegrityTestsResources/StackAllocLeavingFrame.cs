using System.Runtime.CompilerServices;

using V2.Fx;

public static class BetterReadOnlySpanMemoryIntegrityTestsResourcesStackAllocLeavingFrame
{
    private static BetterReadOnlySpan<string> Method()
    {
        ByteSpan span = stackalloc byte[Unsafe.SizeOf<string>()];
        var betterspan = BetterReadOnlySpan<string>.Create(span, 1);

        return betterspan;
    }
}
