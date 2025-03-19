using System;

using V2.Fx.Collections;

public static class AppendedListLeavingFrame
{
    public static LinkedList<int> Method()
    {
        Span<byte> memory = stackalloc byte[LinkedList<int>.MemorySize];
        var list = new LinkedList<int>(42, memory);

        memory = stackalloc byte[LinkedList<int>.MemorySize];
        list.Append(67, memory);

        return list;
    }
}
