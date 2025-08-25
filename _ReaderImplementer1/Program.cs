namespace _ReaderImplementer1
{
    using System.Threading.Tasks;

    using Odata;

    internal class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("success!");
        }
    }

    public sealed class EntityIdHeaderValueReader<TNextReader> : IEntityIdHeaderValueReader<Version.V1, TNextReader>
    {
        private readonly IEntityIdHeaderValueReader<Version.V1, TNextReader> delegateReader;

        public EntityIdHeaderValueReader(IEntityIdHeaderValueReader<Version.V1, TNextReader> delegateReader)
        {
            this.delegateReader = delegateReader;
        }

        public Version.V1 Version { get; } = Odata.Version.V1.Instance;

        public async ValueTask Read()
        {
            await this.delegateReader.Read().ConfigureAwait(false);
        }

        public IEntityIdReader<Version.V1, TNextReader> TryMoveNext(out bool moved)
        {
            //// TODO you are here
            this.delegateReader = 
        }

        private sealed class EntityIdReader : IEntityIdReader<Version.V1, TNextReader>
        {
            private readonly IEntityIdReader<Version.V1, TNextReader> delegateReader;

            public EntityIdReader(IEntityIdReader<Version.V1, TNextReader> delegateReader)
            {
                this.delegateReader = delegateReader;
            }

            public Version.V1 Version => throw new System.NotImplementedException();

            public ValueTask Read()
            {
                throw new System.NotImplementedException();
            }

            public EntityId TryGetValue(out bool moved)
            {
                throw new System.NotImplementedException();
            }

            public TNextReader TryMoveNext(out bool moved)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
