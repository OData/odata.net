using V2.Fx;
using V2.Fx.Collections;

public static class EmptyListLeavingFrame
{
    public static LinkedList<int> Method()
    {
        var list = new LinkedList<int>(stackalloc byte[0]);

        return list;
    }
}
