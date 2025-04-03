using System.Runtime.CompilerServices;

using V2.Fx;

public static class SpanExMemoryIntegrityTestsResourcesStackAllocWithinFrame
{
    public static void Method()
    {
        ByteSpan span = stackalloc byte[Unsafe.SizeOf<string>()];
        var betterSpan = SpanEx<string>.Create(span, 1);
    }
}
