using V2.Fx;
using V2.Fx.Collections;

public static class LoopedAppendedListLeavingFrame
{
    public static LinkedList<int> Method()
    {
        var list = new LinkedList<int>(stackalloc byte[0]);

        ByteSpan memory = stackalloc byte[list.MemorySize];
        list.Append(42, memory);

        for (int i = 0; i < 10; ++i)
        {
            ByteSpan memory2 = stackalloc byte[list.MemorySize];
            list.Append(i, memory2);
        }

        return list;
    }
}
