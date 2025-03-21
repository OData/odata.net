using System;
using V2.Fx;
using V2.Fx.Collections;

public static class SingleValueLinkedListLeavingFrame
{
    public static LinkedList<int> Method()
    {
        DifferentMemory span = stackalloc byte[LinkedList<int>.MemorySize];
        var list = new LinkedList<int>(42, span);
        return list;
    }
}
