using System.Runtime.CompilerServices;

using V2.Fx;

public static class SpanExFactoryMemoryIntegrityTestsResourcesStackAllocWithinFrame
{
    public static void Method()
    {
        ByteSpan span = stackalloc byte[Unsafe.SizeOf<string>()];
        var betterSpan = SpanEx.FromMemory<string>(span, 1);
    }
}
