namespace Odata
{
    using System;

    public interface IOdataReader<out TVersion, out TNext> : IReader<TNext>
        where TVersion : Version.V1
    {
        TVersion Version { get; }
    }

    public interface IOdataReader<out TVersion, out TValue, out TNext> : IOdataReader<TVersion, TNext>, IReader<TValue, TNext>
        where TVersion : Version.V1
    {
    }

    public abstract class Version
    {
        private Version()
        {
        }

        public abstract class V1 : Version
        {
            protected internal V1()
            {
            }

            internal abstract void NoImplementationV1();

            private sealed class Impl : V1
            {
                private Impl()
                {
                }

                public static new Impl Instance { get; } = new Impl();

                internal override void NoImplementationV1()
                {
                    throw new NotImplementedException();
                }
            }

            public static V1 Instance { get; } = Impl.Instance;
        }

        public abstract class V2 : V1
        {
            private readonly IV2Implementations v2Implementations;

            private V2(IV2Implementations v2Implementations)
            {
                this.v2Implementations = v2Implementations;
            }

            private sealed class Impl : V2
            {
                public Impl(IV2Implementations v2Implementations)
                    : base(v2Implementations)
                {
                }

                internal override void NoImplementationV1()
                {
                    throw new NotImplementedException();
                }

                internal override void NoImplementationV2()
                {
                    throw new NotImplementedException();
                }
            }

            public static V2 Create(IV2Implementations v2Implementations)
            {
                return new Impl(v2Implementations);
            }

            internal abstract void NoImplementationV2();

            public bool TryMoveNext<TVersion, TNextReader>(IEntityIdHeaderValueReader<TVersion, TNextReader> entityIdHeaderValueReader, out IEntityIdStartReader<TVersion, TNextReader> entityIdStartReader)
                where TVersion : Version.V2
            {
                return this.v2Implementations.TryMoveNext(entityIdHeaderValueReader, out entityIdStartReader);
            }
        }
    }

    public interface IV2Implementations
    {
        bool TryMoveNext<TVersion, TNextReader>(IEntityIdHeaderValueReader<TVersion, TNextReader> entityIdHeaderValueReader, out IEntityIdStartReader<TVersion, TNextReader> entityIdStartReader)
            where TVersion : Version.V2;
    }
}
