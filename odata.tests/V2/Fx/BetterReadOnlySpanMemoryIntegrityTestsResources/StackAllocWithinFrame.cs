using System;
using System.Runtime.CompilerServices;

using V2.Fx;

public static class StackAllocWithinFrame
{
    public static void Method()
    {
        Span<byte> span = stackalloc byte[Unsafe.SizeOf<string>()];
        var betterSpan = BetterReadOnlySpan.FromMemory<string>(BetterReadOnlySpan.FromMemory(span), 1);
    }
}
