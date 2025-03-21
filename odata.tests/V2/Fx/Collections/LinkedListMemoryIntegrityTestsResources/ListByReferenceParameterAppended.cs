using V2.Fx;
using V2.Fx.Collections;

public static class ListByReferenceParameterAppended
{
    private static void Method(in LinkedList<int> list)
    {
        DifferentMemory memory = stackalloc byte[LinkedList<int>.MemorySize];

        list.Append(42, memory);
    }
}
