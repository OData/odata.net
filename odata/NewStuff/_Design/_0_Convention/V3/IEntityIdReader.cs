using System.Threading.Tasks;

namespace NewStuff._Design._0_Convention.V3
{
    //// TODO you are in 4.1 of this doc: https://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html
    //// you are trying to decide if you should go ahead and dive into iri parsing (and if you don't, what will it look like when you decide that you want to, because up to this point, you would have called the entry point reader for the "broken down" version `ientityidreader`, but then that would conflict with the current name); and also, how it's going to look to write an `entityid` when you can only write uris and not iris; basically, what should you do when reading and writing are not symmetric

    public interface IEntityIdHeaderReader<out TNextReader> : IReader<EntityIdHeader, TNextReader>
    {
    }

    public sealed class EntityIdHeader
    {
        internal EntityIdHeader(string headerName, EntityId headerValue)
        {
            HeaderName = headerName;
            HeaderValue = headerValue;
        }

        public string HeaderName { get; }

        public EntityId HeaderValue { get; }
    }

    public static partial class Extensions
    {
        public static IEntityIdHeaderReader<TNextReader> ToEntityIdHeaderReader<TNextReader>(
            this IEntityIdHeaderStartReader<TNextReader> entityIdHeaderStartReader)
        {
            return new EntityIdHeaderReader<TNextReader>(entityIdHeaderStartReader);
        }

        private sealed class EntityIdHeaderReader<TNextReader> : IEntityIdHeaderReader<TNextReader>
        {
            private readonly IEntityIdHeaderStartReader<TNextReader> entityIdHeaderStartReader;

            public EntityIdHeaderReader(IEntityIdHeaderStartReader<TNextReader> entityIdHeaderStartReader)
            {
                this.entityIdHeaderStartReader = entityIdHeaderStartReader;
            }

            public ValueTask Read()
            {
                throw new System.NotImplementedException();
            }

            public EntityIdHeader TryGetValue(out bool moved)
            {
                throw new System.NotImplementedException();
            }

            public TNextReader TryMoveNext(out bool moved)
            {
                throw new System.NotImplementedException();
            }
        }
    }

    public interface IEntityIdHeaderStartReader<out TNextReader> : IReader<IEntityIdHeaderNameReader<TNextReader>>
    {
    }

    public interface IEntityIdHeaderNameReader<out TNextReader> : IReader<EntityIdHeaderName, IEntityIdHeaderValueReader<TNextReader>>
    {
    }

    public sealed class EntityIdHeaderName
    {
        private EntityIdHeaderName()
        {
        }
    }

    public interface IEntityIdHeaderValueReader<out TNextReader> : IReader<IEntityIdStartReader<TNextReader>>
    {
    }

    public interface IEntityIdReader<out TNextReader> : IReader<EntityId, TNextReader>
    {
    }

    public sealed class EntityId
    {
        private EntityId()
        {
        }
    }

    public interface IEntityIdStartReader<out TNextReader> : IReader<IIriSchemeReader<TNextReader>>
    {
    }

    public interface IIriSchemeReader<out TNextReader>
    {
    }







    namespace V1
    {
        public interface IEntityIdHeaderValueReaderV1<out TNextReader> : IReader<IEntityIdReaderV1<TNextReader>>
        {
        }

        public interface IEntityIdReaderV1<out TNextReader> : IReader<EntityId, TNextReader>
        {
        }
    }





    public interface IEntityIdHeaderValueReaderV2<out TNextReader> : IReader<IEntityIdStartReaderV2<TNextReader>>
    {
    }

    public interface IEntityIdStartReaderV2<out TNextReader> : IReader<IIriSchemeReaderV2<TNextReader>>
    {
    }

    public interface IIriSchemeReaderV2<out TNextReader> : IReader<TNextReader>
    {
    }

















    public readonly ref struct EntityIdHeaderValueReader<TNextReader>
    {
    }

    public interface IEntityIdHeaderValueReaderV1<out TNextReader> : IReader<IEntityIdReaderV1<TNextReader>>
    {
    }

    public interface IEntityIdReaderV1<out TNextReader> : IReader<EntityId, TNextReader>
    {
    }
}
