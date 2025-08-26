namespace _ReaderImplementer1
{
    using System.Threading.Tasks;

    using Odata;

    internal class Program
    {
        static async Task Main(string[] args)
        {
            var entityIdHeaderValueReader = new EntityIdHeaderValueReader<object>(new ImplementationV1.EntityIdHeaderValueReader<object>());
            var nextReader = await Read(entityIdHeaderValueReader).ConfigureAwait(false);
            System.Console.WriteLine("success!");
        }

        public static async Task<TNextReader> Read<TNextReader>(IEntityIdHeaderValueReader<Version.V1, TNextReader> entityIdHeaderValueReader)
        {
            IEntityIdReader<Version.V1, TNextReader> entityIdReader;
            while (!entityIdHeaderValueReader.TryMoveNext2(out entityIdReader))
            {
                await entityIdHeaderValueReader.Read().ConfigureAwait(false);
            }

            TNextReader next;
            while (!entityIdReader.TryMoveNext2(out next))
            {
                await entityIdReader.Read().ConfigureAwait(false);
            }

            return next;
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
