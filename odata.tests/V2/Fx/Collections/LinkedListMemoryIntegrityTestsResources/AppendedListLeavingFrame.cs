using V2.Fx;
using V2.Fx.Collections;

public static class AppendedListLeavingFrame
{
    public static LinkedList<int> Method()
    {
        DifferentMemory memory = stackalloc byte[LinkedList<int>.MemorySize];
        var list = new LinkedList<int>(42, memory);

        memory = stackalloc byte[LinkedList<int>.MemorySize];
        list.Append(67, memory);

        return list;
    }
}
