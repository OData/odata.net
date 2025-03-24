using V2.Fx;

public static class WrappedValueLeavingFrame
{
    public static Wrapper<int> Method()
    {
        var value = 42;
        var wrapper = new Wrapper<int>(BetterReadOnlySpan.FromInstance(ref value));
        return wrapper;
    }

    public readonly ref struct Wrapper<T>
    {
        private readonly BetterReadOnlySpan<T> span;

        public Wrapper(BetterReadOnlySpan<T> span)
        {
            this.span = span;
        }
    }
}
