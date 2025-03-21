using System.Runtime.CompilerServices;

using V2.Fx;

public static class StackAllocWithinFrame
{
    public static void Method()
    {
        ByteSpan span = stackalloc byte[Unsafe.SizeOf<string>()];
        var betterSpan = BetterReadOnlySpan.FromMemory<string>(span, 1);
    }
}
