using System;
using V2.Fx;
using V2.Fx.Collections;

public static class ListParameterAppended
{
    private static void Method(LinkedList<int> list)
    {
        Span<byte> memory = stackalloc byte[LinkedList<int>.MemorySize];

        var differentMemory = DifferentMemory.Create(memory);

        list.Append(42, differentMemory);
    }
}
