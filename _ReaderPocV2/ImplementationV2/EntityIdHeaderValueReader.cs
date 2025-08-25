namespace ImplementationV2
{
    using System.Threading.Tasks;

    using Odata;

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

        public bool TryMoveNext(out IEntityIdStartReader<Version.V2, TNextReader> entityIdStartReader)
        {
            entityIdStartReader = new EntityIdStartReader();
            return true;
        }

        private sealed class EntityIdStartReader : IEntityIdStartReader<Version.V2, TNextReader>
        {
            public Version.V2 Version { get; } = V2Implementations.Instance;

            public async ValueTask Read()
            {
                await ValueTask.CompletedTask.ConfigureAwait(false);
            }

            public IIriSchemeReader<Version.V2, TNextReader> TryMoveNext(out bool moved)
            {
                moved = true;
                return new IriSchemeReader();
            }

            private sealed class IriSchemeReader : IIriSchemeReader<Version.V2, TNextReader>
            {
                public Version.V2 Version { get; } = V2Implementations.Instance;

                public async ValueTask Read()
                {
                    await ValueTask.CompletedTask.ConfigureAwait(false);
                }

                public IriScheme TryGetValue(out bool moved)
                {
                    moved = true;
                    return default;
                }

                public TNextReader TryMoveNext(out bool moved)
                {
                    moved = true;
                    return default;
                }
            }
        }

        public Version.V2 Version { get; } = V2Implementations.Instance;

        public async ValueTask Read()
        {
            await ValueTask.CompletedTask.ConfigureAwait(false);
        }

        public IEntityIdReader<Version.V2, TNextReader> TryMoveNext(out bool moved)
        {
            moved = true;
            return new EntityIdReader();
        }

        private sealed class EntityIdReader : IEntityIdReader<Version.V2, TNextReader>
        {
            public Version.V2 Version { get; } = V2Implementations.Instance;

            public async ValueTask Read()
            {
                await ValueTask.CompletedTask.ConfigureAwait(false);
            }

            public EntityId TryGetValue(out bool moved)
            {
                moved = true;
                return default;
            }

            public TNextReader TryMoveNext(out bool moved)
            {
                moved = true;
                return default;
            }
        }
    }
}
