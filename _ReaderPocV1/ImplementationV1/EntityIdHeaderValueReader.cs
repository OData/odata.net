namespace ImplementationV1
{
    using System.Threading.Tasks;

    using Odata;

    public sealed class EntityIdHeaderValueReader<TNextReader> : IEntityIdHeaderValueReader<Version.V1, TNextReader>
    {
        public Version.V1 Version
        {
            get
            {
                return Odata.Version.V1.Instance;
            }
        }

        public async ValueTask Read()
        {
            await ValueTask.CompletedTask.ConfigureAwait(false);
        }

        public IEntityIdReader<Version.V1, TNextReader> TryMoveNext(out bool moved)
        {
            moved = true;
            return new EntityIdReader();
        }

        private sealed class EntityIdReader : IEntityIdReader<Version.V1, TNextReader>
        {
            public Version.V1 Version
            {
                get
                {
                    return Odata.Version.V1.Instance;
                }
            }

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
