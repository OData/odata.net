using System;
using System.Runtime.CompilerServices;

using V2.Fx.Collections;

public static class SingleValueLinkedListLeavingFrame
{
    public static LinkedList<int> Method()
    {
        Span<byte> span = stackalloc byte[Unsafe.SizeOf<int>()];
        var list = new LinkedList<int>(42, span);
        return list;
    }
}
