namespace odata.tests
{
    using System.Collections;

    class Derived : NewStuff._Design._0_Convention.V3.Attempt3.V1.Version.V1
    {
        internal override void NoImplementation()
        {
            throw new NotImplementedException();
        }
    }

    public ref struct Foo : IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    public static class FooExt
    {
        public static void Frob<T>(this T disposable) where T : IDisposable, allows ref struct
        {
            disposable.Dispose();
        }

        public static void DoWork()
        {
            var foo = new Foo();
            foo.Frob();
        }
    }

    public ref struct EnumerableWrapper<TEnumerable, TElement> where TEnumerable : IEnumerable<TElement>, allows ref struct
    {
        public EnumerableWrapper(TEnumerable enumerable)
        {
            Enumerable = enumerable;
        }

        public TEnumerable Enumerable { get; }
    }

    public ref struct FakeEnumerable : IEnumerable<string>
    {
        public IEnumerator<string> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    public interface IEnumerable<out TEnumerable, out TElement> : IEnumerable<TElement> 
        where TEnumerable : IEnumerable<TEnumerable, TElement>, IEnumerable<TElement>, allows ref struct 
        where TElement : allows ref struct
    {
    }

    public ref struct Thing<TElement> : IEnumerable<Thing<TElement>, TElement> where TElement : allows ref struct
    {
        private readonly IEnumerable<TElement> source;

        public Thing(IEnumerable<TElement> source)
        {
            this.source = source;
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    public static class Thing2<TElement> where TElement : allows ref struct
    {
        public static Thing<TElement> Wrap<TEnumerable>(TEnumerable source)
            where TEnumerable : IEnumerable<TElement>, allows ref struct
        {
            return new Thing<TElement>();
        }
    }

    public static class EnumerableWrapperExt
    {
        public static bool Any<TElement>(this Thing<TElement> enumerable)
        {
            return true;
        }

        public static Thing<TElement> Wrap<TElement>(this IEnumerable<TElement> source)
        {
            return new Thing<TElement>(source);
        }

        public static void DoWork()
        {
            var stuff = new FakeEnumerable();
            var thing = Thing2<string>.Wrap(stuff);
            thing.Any();
        }
    }
}
