using System.Runtime.CompilerServices;

using V2.Fx;

public static class SpanExMemoryIntegrityTestsResourcesStackAllocLeavingFrame
{
    private static SpanEx<string> Method()
    {
        ByteSpan span = stackalloc byte[Unsafe.SizeOf<string>()];
        var betterspan = SpanEx<string>.Create(span, 1);

        return betterspan;
    }
}
