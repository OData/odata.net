using V2.Fx;
using V2.Fx.Collections;

public static class ListParameterAppended
{
    private static void Method(LinkedList<int> list)
    {
        ByteSpan memory = stackalloc byte[list.MemorySize];

        list.Append(42, memory);
    }
}
