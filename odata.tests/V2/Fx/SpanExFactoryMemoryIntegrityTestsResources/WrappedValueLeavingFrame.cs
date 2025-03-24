using V2.Fx;

public static class SpanExFactoryMemoryIntegrityTestsResourcesWrappedValueLeavingFrame
{
    public static Wrapper<int> Method()
    {
        var value = 42;
        var wrapper = new Wrapper<int>(SpanEx.FromInstance(ref value));
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
