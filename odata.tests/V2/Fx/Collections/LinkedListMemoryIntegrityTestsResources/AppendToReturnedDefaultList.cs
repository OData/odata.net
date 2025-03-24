using V2.Fx;
using V2.Fx.Collections;

public static class AppendToReturnedDefaultList
{
    public static void Method()
    {
        var list = GetList();

        ByteSpan memory = stackalloc byte[list.MemorySize];
        list.Append(42, memory);
    }

    private static LinkedList<int> GetList()
    {
        return new LinkedList<int>();
    }
}
