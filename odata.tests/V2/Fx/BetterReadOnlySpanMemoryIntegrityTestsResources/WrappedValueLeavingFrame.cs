using V2.Fx;

public static class BetterReadOnlySpanMemoryIntegrityTestsResourcesWrappedValueLeavingFrame
{
    public static Wrapper<int> Method()
    {
        var value = 42;
        var wrapper = new Wrapper<int>(BetterReadOnlySpan.FromInstance(ref value));
        return wrapper;
    }

    public readonly ref struct Wrapper<T>
    {
        private readonly SpanEx<T> span;

        public Wrapper(SpanEx<T> span)
        {
            this.span = span;
        }
    }
}
