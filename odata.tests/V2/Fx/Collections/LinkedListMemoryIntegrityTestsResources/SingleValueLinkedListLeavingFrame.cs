using V2.Fx;
using V2.Fx.Collections;

public static class SingleValueLinkedListLeavingFrame
{
    public static LinkedList<int> Method()
    {
        var list = new LinkedList<int>(stackalloc byte[0]);

        ByteSpan span = stackalloc byte[list.MemorySize];
        list.Append(42, span);
        return list;
    }
}
