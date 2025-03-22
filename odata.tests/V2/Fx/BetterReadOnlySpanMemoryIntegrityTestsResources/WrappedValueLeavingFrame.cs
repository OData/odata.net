using V2.Fx;

public static class WrappedValueLeavingFrame
{
    public static Wrapper<int> Method()
    {
        var wrapper = new Wrapper<int>(BetterReadOnlySpan.FromInstance(42));
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
