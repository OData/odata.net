using System;
using V2.Fx;
using V2.Fx.Collections;

public static class LoopedAppendedListLeavingFrame
{
    public static LinkedList<int> Method()
    {
        DifferentMemory memory = stackalloc byte[LinkedList<int>.MemorySize];
        var list = new LinkedList<int>(42, memory);

        for (int i = 0; i < 10; ++i)
        {
            DifferentMemory memory2 = stackalloc byte[LinkedList<int>.MemorySize];
            list.Append(i, memory2);
        }

        return list;
    }
}
