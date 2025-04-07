using V2.Fx;
using V2.Fx.Collections;

public static class AppendedListLeavingFrame
{
    public static LinkedList<int> Method()
    {
        var list = new LinkedList<int>(stackalloc byte[0]);

        ByteSpan memory = stackalloc byte[list.MemorySize];
        list.Append(42, memory);

        memory = stackalloc byte[list.MemorySize];
        list.Append(67, memory);

        return list;
    }
}
