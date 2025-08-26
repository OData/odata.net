namespace _ReaderImplementer2
{
    using System.Threading.Tasks;

    using Odata;

    internal class Program
    {
        static async Task Main(string[] args)
        {
            var entityIdHeaderValueReader = new EntityIdHeaderValueReader<object>(new ImplementationV2.EntityIdHeaderValueReader<object>());
            var nextReader = await Read(entityIdHeaderValueReader).ConfigureAwait(false);
            System.Console.WriteLine("success!");
        }

        public static async Task<TNextReader> Read<TNextReader>(IEntityIdHeaderValueReader<Version.V2, TNextReader> entityIdHeaderValueReader)
        {
            IEntityIdStartReader<Version.V2, TNextReader> entityIdStartReader;
            while (!entityIdHeaderValueReader.Version.TryMoveNext(entityIdHeaderValueReader, out entityIdStartReader))
            {
                await entityIdHeaderValueReader.Read().ConfigureAwait(false);
            }

            IIriSchemeReader<Version.V2, TNextReader> iriSchemeReader;
            while (!entityIdStartReader.TryMoveNext2(out iriSchemeReader))
            {
                await entityIdStartReader.Read().ConfigureAwait(false);
            }

            TNextReader nextReader;
            while (!iriSchemeReader.TryMoveNext2(out nextReader))
            {
                await iriSchemeReader.Read().ConfigureAwait(false);
            }

            return nextReader;
        }
    }

    public sealed class EntityIdHeaderValueReader<TNextReader> : IEntityIdHeaderValueReader<Version.V2, TNextReader>
    {
        private sealed class V2Implementations : IV2Implementations
        {
            private V2Implementations()
            {
            }

            public static Version.V2 Instance { get; } = Odata.Version.V2.Create(new V2Implementations());

            public bool TryMoveNext<TVersion, TNextReader1>(
                IEntityIdHeaderValueReader<TVersion, TNextReader1> entityIdHeaderValueReader,
                out IEntityIdStartReader<Version.V2, TNextReader1> entityIdStartReader)
                where TVersion : Version.V2
            {
                if (!(entityIdHeaderValueReader is EntityIdHeaderValueReader<TNextReader1> v2EntityIdHeaderValueReader))
                {
                    throw new System.Exception("TODO make this a custom exception");
                }

                return v2EntityIdHeaderValueReader.TryMoveNext(out entityIdStartReader);
            }
        }

        private readonly IEntityIdHeaderValueReader<Version.V2, TNextReader> delegateReader;

        public EntityIdHeaderValueReader(IEntityIdHeaderValueReader<Version.V2, TNextReader> delegateReader)
        {
            this.delegateReader = delegateReader;
        }

        public bool TryMoveNext(out IEntityIdStartReader<Version.V2, TNextReader> entityIdStartReader)
        {
            if (!this.delegateReader.Version.TryMoveNext(this.delegateReader, out var delegateEntityIdStartReader))
            {
                entityIdStartReader = default;
                return false;
            }

            entityIdStartReader = new EntityIdStartReader(delegateEntityIdStartReader);
            return true;
        }

        private sealed class EntityIdStartReader : IEntityIdStartReader<Version.V2, TNextReader>
        {
            private readonly IEntityIdStartReader<Version.V2, TNextReader> delegateReader;

            public EntityIdStartReader(IEntityIdStartReader<Version.V2, TNextReader> delegateReader)
            {
                this.delegateReader = delegateReader;
            }

            public Version.V2 Version { get; } = V2Implementations.Instance;

            public async ValueTask Read()
            {
                await this.delegateReader.Read().ConfigureAwait(false);
            }

            public IIriSchemeReader<Version.V2, TNextReader> TryMoveNext(out bool moved)
            {
                var delegatedIriSchemeReader = this.delegateReader.TryMoveNext(out moved);
                if (!moved)
                {
                    return delegatedIriSchemeReader;
                }

                return new IriSchemeReader(delegatedIriSchemeReader);
            }

            private sealed class IriSchemeReader : IIriSchemeReader<Version.V2, TNextReader>
            {
                private readonly IIriSchemeReader<Version.V2, TNextReader> delegateReader;

                public IriSchemeReader(IIriSchemeReader<Version.V2, TNextReader> delegateReader)
                {
                    this.delegateReader = delegateReader;
                }

                public Version.V2 Version { get; } = V2Implementations.Instance;

                public async ValueTask Read()
                {
                    await this.delegateReader.Read().ConfigureAwait(false);
                }

                public IriScheme TryGetValue(out bool moved)
                {
                    var iriScheme = this.delegateReader.TryGetValue(out moved);
                    if (!moved)
                    {
                        return iriScheme;
                    }

                    return default; //// TODO the idea is that some transform happens here
                }

                public TNextReader TryMoveNext(out bool moved)
                {
                    return this.delegateReader.TryMoveNext(out moved);
                }
            }
        }

        public Version.V2 Version { get; } = V2Implementations.Instance;

        public async ValueTask Read()
        {
            await this.delegateReader.Read().ConfigureAwait(false);
        }

        public IEntityIdReader<Version.V2, TNextReader> TryMoveNext(out bool moved)
        {
            var delegatedEntityIdReader = this.delegateReader.TryMoveNext(out moved);
            if (!moved)
            {
                return delegatedEntityIdReader;
            }

            return new EntityIdReader(delegatedEntityIdReader);
        }

        private sealed class EntityIdReader : IEntityIdReader<Version.V2, TNextReader>
        {
            private readonly IEntityIdReader<Version.V2, TNextReader> delegateReader;

            public EntityIdReader(IEntityIdReader<Version.V2, TNextReader> delegateReader)
            {
                this.delegateReader = delegateReader;
            }

            public Version.V2 Version { get; } = V2Implementations.Instance;

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
