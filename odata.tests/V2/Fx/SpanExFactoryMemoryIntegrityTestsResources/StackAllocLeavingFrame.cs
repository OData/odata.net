using System.Runtime.CompilerServices;

using V2.Fx;

public static class SpanExFactoryMemoryIntegrityTestsResourcesStackAllocLeavingFrame
{
    private static SpanEx<string> Method()
    {
        ByteSpan span = stackalloc byte[Unsafe.SizeOf<string>()];
        var betterspan = SpanEx.FromMemory<string>(span, 1);

        return betterspan;
    }
}
