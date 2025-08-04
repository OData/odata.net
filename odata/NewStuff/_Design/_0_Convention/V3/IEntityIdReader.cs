using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace NewStuff._Design._0_Convention.V3
{
    //// TODO you are in 4.1 of this doc: https://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html
    //// you are trying to decide if you should go ahead and dive into iri parsing (and if you don't, what will it look like when you decide that you want to, because up to this point, you would have called the entry point reader for the "broken down" version `ientityidreader`, but then that would conflict with the current name); and also, how it's going to look to write an `entityid` when you can only write uris and not iris; basically, what should you do when reading and writing are not symmetric

    //// TODO i think the `attempt2` namespace is the right track; finish the extension method, then implement some readers, and then write some fake calling code

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







    namespace Attempt2
    {
        namespace V1
        {
            public interface IEntityIdHeaderValueReader<out TNextReader> : IReader<IEntityIdReader<TNextReader>>
            {
            }

            public interface IEntityIdReader<out TNextReader> : IReader<EntityId, TNextReader>
            {
            }

            public static class Extensions
            {
                public static bool TryToV2<TNextReader>(this IEntityIdHeaderValueReader<TNextReader> reader, [MaybeNullWhen(false)] out V2.IEntityIdHeaderValueReader<TNextReader> v2)
                {
                    ArgumentNullException.ThrowIfNull(reader);

                    return (v2 = reader as V2.IEntityIdHeaderValueReader<TNextReader>) != null;
                }
            }
        }

        namespace V2
        {
            public sealed class EntityIdHeaderValueReader<TNextReader> : IEntityIdHeaderValueReader<TNextReader>
            {
                public ValueTask Read()
                {
                    throw new NotImplementedException();
                }

                public IEntityIdStartReader<TNextReader> TryMoveNext(out bool moved)
                {
                    throw new NotImplementedException();
                }

                V1.IEntityIdReader<TNextReader> IReader<V1.IEntityIdReader<TNextReader>>.TryMoveNext(out bool moved)
                {
                    return this.ToV1<TNextReader>().TryMoveNext(out moved);
                }
            }

            public interface IEntityIdHeaderValueReader<out TNextReader> : IReader<IEntityIdStartReader<TNextReader>>, IReader<V1.IEntityIdReader<TNextReader>>
            {
            }

            public interface IEntityIdStartReader<out TNextReader> : IReader<IIriSchemeReader<TNextReader>>
            {
            }

            public interface IIriSchemeReader<out TNextReader> : IReader<TNextReader>
            {
            }

            public static class Extensions
            {
                public static IReader<V1.IEntityIdReader<TNextReader>> ToV1<TNextReader>(this IReader<IEntityIdStartReader<TNextReader>> reader)
                {
                    return new EntityIdHeaderValueReader<TNextReader>(reader);
                }

                private sealed class EntityIdHeaderValueReader<TNextReader> : IReader<V1.IEntityIdReader<TNextReader>>
                {
                    private readonly IReader<IEntityIdStartReader<TNextReader>> reader;

                    public EntityIdHeaderValueReader(IReader<IEntityIdStartReader<TNextReader>> reader)
                    {
                        this.reader = reader;
                    }

                    public ValueTask Read()
                    {
                        return this.reader.Read();
                    }

                    public V1.IEntityIdReader<TNextReader> TryMoveNext(out bool moved)
                    {
                        if (!this.reader.TryMoveNext2(out var iriSchemeReader))
                        {
                            moved = false;
                            return default;
                        }

                        throw new NotImplementedException();
                    }

                    private sealed class EntityIdReader : V1.IEntityIdReader<TNextReader>
                    {
                        public EntityIdReader()

                        public ValueTask Read()
                        {
                            throw new NotImplementedException();
                        }

                        public EntityId TryGetValue(out bool moved)
                        {
                            throw new NotImplementedException();
                        }

                        public TNextReader TryMoveNext(out bool moved)
                        {
                            throw new NotImplementedException();
                        }
                    }
                }
            }
        }
    }
}
