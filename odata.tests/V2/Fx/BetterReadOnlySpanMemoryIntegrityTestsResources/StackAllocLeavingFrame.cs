using System.Runtime.CompilerServices;

using V2.Fx;

public static class StackAllocLeavingFrame
{
    private static BetterReadOnlySpan<string> Method()
    {
        DifferentMemory span = stackalloc byte[Unsafe.SizeOf<string>()];
        var betterspan = BetterReadOnlySpan.FromMemory<string>(span, 1);

        return betterspan;
    }
}
