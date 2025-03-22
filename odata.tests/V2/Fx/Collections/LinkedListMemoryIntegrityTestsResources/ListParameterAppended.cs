using V2.Fx;
using V2.Fx.Collections;

public static class ListParameterAppended
{
    private static void Method(LinkedList<int> list)
    {
        ByteSpan memory = stackalloc byte[LinkedList<int>.MemorySize];

        list.Append(42, memory);
    }
}
