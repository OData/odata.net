using System;
using System.Runtime.CompilerServices;

using V2.Fx;

public static class StackAllocLeavingFrame
{
    private static BetterReadOnlySpan<string> Method()
    {
        Span<byte> span = stackalloc byte[Unsafe.SizeOf<string>()];
        var betterspan = BetterReadOnlySpan.FromMemory<string>(span, 1);

        //// THIS IS A GOOD THING
        return betterspan;
    }
}
