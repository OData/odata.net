using System;
using V2.Fx;
using V2.Fx.Collections;

public static class ListByReferenceParameterAppended
{
    private static void Method(in LinkedList<int> list)
    {
        Span<byte> memory = stackalloc byte[LinkedList<int>.MemorySize];

        var differentMemory = DifferentMemory.Create(memory);

        list.Append(42, differentMemory);
    }
}
