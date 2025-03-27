using V2.Fx;
using V2.Fx.Collections;

public static class AppendToDefaultList
{
    public static void Method()
    {
        var list = new LinkedList<int>();

        ByteSpan memory = stackalloc byte[list.MemorySize];
        list.Append(42, memory);
    }
}
