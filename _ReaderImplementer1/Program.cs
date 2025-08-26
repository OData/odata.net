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
            var delegatedEntityIdReader = this.delegateReader.TryMoveNext(out moved);
            if (!moved)
            {
                return delegatedEntityIdReader;
            }

            return new EntityIdReader(delegatedEntityIdReader);
        }

        private sealed class EntityIdReader : IEntityIdReader<Version.V1, TNextReader>
        {
            private readonly IEntityIdReader<Version.V1, TNextReader> delegateReader;

            public EntityIdReader(IEntityIdReader<Version.V1, TNextReader> delegateReader)
            {
                this.delegateReader = delegateReader;
            }

            public Version.V1 Version { get; } = Odata.Version.V1.Instance;

            public async ValueTask Read()
            {
                await this.delegateReader.Read().ConfigureAwait(false);
            }

            public EntityId TryGetValue(out bool moved)
            {
                var entityId = this.delegateReader.TryGetValue(out moved);
                if (!moved)
                {
                    return entityId;
                }

                return default; //// TODO the idea is that some transform happens here
            }

            public TNextReader TryMoveNext(out bool moved)
            {
                return this.delegateReader.TryMoveNext(out moved);
            }
        }
    }
}
