namespace Odata
{
    public interface IOdataReader<out TVersion, out TNext> : IReader<TNext>
    {
        TVersion Version { get; }
    }

    public interface IOdataReader<out TVersion, out TValue, out TNext> : IOdataReader<TVersion, TNext>, IReader<TValue, TNext>
    {
    }

    public abstract class Version
    {
        private Version()
        {
        }

        public abstract class V1 : Version
        {
            private V1()
            {
            }

            internal abstract void NoImplementation();

            private sealed class Impl : V1
            {
                private Impl()
                {
                }

                public static new Impl Instance { get; } = new Impl();

                internal override void NoImplementation()
                {
                    throw new NotImplementedException();
                }
            }

            public static V1 Instance { get; } = Impl.Instance;
        }
    }
}
