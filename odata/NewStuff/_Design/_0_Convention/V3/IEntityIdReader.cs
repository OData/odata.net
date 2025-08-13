using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

using NewStuff._Design._0_Convention.V3.Attempt2.V2;
using NewStuff._Design._0_Convention.V3.Attempt3.V2;

namespace NewStuff._Design._0_Convention.V3
{
    //// TODO you are in 4.1 of this doc: https://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html
    //// you are trying to decide if you should go ahead and dive into iri parsing (and if you don't, what will it look like when you decide that you want to, because up to this point, you would have called the entry point reader for the "broken down" version `ientityidreader`, but then that would conflict with the current name); and also, how it's going to look to write an `entityid` when you can only write uris and not iris; basically, what should you do when reading and writing are not symmetric

    //// TODO i think the `attempt2` namespace is the right track; finish the extension method, then implement some readers, and then write some fake calling code


    public static class ForMike
    {
        public struct Nothing
        {
        }

        public interface IGetResponseReader : IReader<IEntityIdHeaderReader<IGetResponseBodyReader>>
        {
        }

        public interface IGetResponseBodyReader
        {
        }

        public static async Task Main(IGetResponseReader reader)
        {
            reader.TryMoveNext2(out var entityIdHeaderReader);

            EntityIdHeader entityIdHeader;
            while (!entityIdHeaderReader.TryGetValue2(out entityIdHeader))
            {
                await reader.Read().ConfigureAwait(false);
            }

            Console.WriteLine(entityIdHeader.HeaderName);

            while (!entityIdHeaderReader.TryMoveNext2(out var getResponseBodyReader))
            {
                await reader.Read().ConfigureAwait(false);
            }

            // do something with getResponseBodyReader
        }

        public static async Task<TNextReader> Main<TNextReader>(IEntityIdHeaderReader<TNextReader> reader)
        {
            EntityIdHeader entityIdHeader;
            while (!reader.TryGetValue2(out entityIdHeader))
            {
                await reader.Read().ConfigureAwait(false);
            }

            Console.WriteLine(entityIdHeader.HeaderName);

            reader.TryMoveNext2(out var nextReader);

            return nextReader;
        }
    }

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
                        ////public EntityIdReader()

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

    namespace Attempt3
    {
        namespace V1
        {
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
                }
            }

            public interface IEntityIdHeaderValueReader<out TVersion, out TNextReader> : IReader<IEntityIdReader<TVersion, TNextReader>>
                where TVersion : Version
            {
                IV2Placeholder<TVersion, TNextReader> V2Placeholder { get; }
            }

            public interface IEntityIdReader<out TVersion, out TNextReader> : IReader<EntityId, TNextReader>
                where TVersion : Version
            {
            }

            public interface IV2Placeholder<out TVersion, out TNextReader>
                where TVersion : Version
            {
                internal void NoImplementation();
            }
        }

        namespace V2
        {
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
                }

                public abstract class V2 : V1
                {
                    private V2()
                    {
                    }

                    internal abstract void NoImplementation();
                }
            }

            //// TODO you are here trying to do the version thing; make sure that ireader<v2> can be cast to ireader<v1>; you also thought about exploring if the `v1`, `v2`, etc. classes could have the "get extended reader" methods, so for example, ientityidheadervaluereader would have a `TVersion Version { get; }` and then this could be called to say `reader.Version.GetExtended(entityIdHeaderValueReader)` and it'd give you back the irireader instead of the entityidreader
            public interface IEntityIdHeaderValueReader<out TVersion, out TNextReader> : IReader<IEntityIdReader<TVersion, TNextReader>>
                where TVersion : Version
            {
                /// <summary>
                /// TODO throws
                /// </summary>
                IV2Placeholder<TVersion, TNextReader> V2Placeholder { get; }
            }

            public interface IEntityIdReader<out TVersion, out TNextReader> : IReader<EntityId, TNextReader>
                where TVersion : Version
            {
            }

            public interface IV2Placeholder<out TVersion, out TNextReader> : IReader<IEntityIdStartReader<TVersion, TNextReader>>
                where TVersion : Version
            {
            }

            public interface IEntityIdStartReader<out TVersion, out TNextReader> : IReader<IIriSchemeReader<TVersion, TNextReader>>
                where TVersion : Version
            {
            }

            public interface IIriSchemeReader<out TVersion, out TNextReader> : IReader<IriScheme, TNextReader>
                where TVersion : Version
            {
            }

            public struct IriScheme
            {
            }

            public static class Extensions
            {
                public static IEntityIdHeaderValueReader<Version.V1, TNextReader> ToV1<TVersion, TNextReader>(this IEntityIdHeaderValueReader<TVersion, TNextReader> entityIdHeaderValueReader)
                    where TVersion : Version.V2
                {
                    return new EntityIdHeaderValueReader<TVersion, TNextReader>(entityIdHeaderValueReader.V2Placeholder);
                }

                private sealed class EntityIdHeaderValueReader<TVersion, TNextReader> : IEntityIdHeaderValueReader<Version.V1, TNextReader>
                    where TVersion : Version.V2
                {
                    private readonly IV2Placeholder<TVersion, TNextReader> v2Placeholder;

                    public EntityIdHeaderValueReader(IV2Placeholder<TVersion, TNextReader> v2Placeholder)
                    {
                        this.v2Placeholder = v2Placeholder;
                    }

                    public IV2Placeholder<Version.V1, TNextReader> V2Placeholder
                    {
                        get
                        {
                            return this.v2Placeholder;
                        }
                    }

                    public ValueTask Read()
                    {
                        return ValueTask.CompletedTask;
                    }

                    public IEntityIdReader<Version.V1, TNextReader> TryMoveNext(out bool moved)
                    {
                        moved = true;
                        return new EntityIdReader(this.v2Placeholder);
                    }

                    private sealed class EntityIdReader : IEntityIdReader<TVersion, TNextReader>
                    {
                        private readonly IV2Placeholder<TVersion, TNextReader> v2Placeholder;

                        private IEntityIdStartReader<TVersion, TNextReader>? entityIdStartReader;
                        private IIriSchemeReader<TVersion, TNextReader>? iriSchemeReader;

                        public EntityIdReader(IV2Placeholder<TVersion, TNextReader> v2Placeholder)
                        {
                            this.v2Placeholder = v2Placeholder;
                        }

                        public async ValueTask Read()
                        {
                            if (this.iriSchemeReader != null)
                            {
                                await this.iriSchemeReader.Read().ConfigureAwait(false);
                            }
                            else if (this.entityIdStartReader != null)
                            {
                                await this.entityIdStartReader.Read().ConfigureAwait(false);
                            }
                            else
                            {
                                await this.v2Placeholder.Read().ConfigureAwait(false);
                            }
                        }

                        public EntityId TryGetValue(out bool moved)
                        {
                            if (this.entityIdStartReader == null)
                            {
                                this.entityIdStartReader = this.v2Placeholder.TryMoveNext(out moved);
                                if (!moved)
                                {
                                    return default;
                                }
                            }

                            if (this.iriSchemeReader == null)
                            {
                                this.iriSchemeReader = entityIdStartReader.TryMoveNext(out moved);
                                if (!moved)
                                {
                                    return default;
                                }
                            }

                            var iriScheme = this.iriSchemeReader.TryGetValue(out moved);
                            return default; // TODO create entity id from irischeme and other iri types
                        }

                        public TNextReader TryMoveNext(out bool moved)
                        {
                            if (this.iriSchemeReader == null)
                            {
                                this.TryGetValue(out moved);
                                if (!moved)
                                {
                                    return default;
                                }
                            }

                            //// TODO i think you can mark up `trygetvalue` to show that `irischemereader` is not null here
                            return this.iriSchemeReader.TryMoveNext(out moved);
                        }
                    }
                }
            }
        }


        namespace ReaderCaller
        {
            namespace OnlyV1IsReleased
            {
                public static class Play
                {
                    public static TNextReader Read<TNextReader>(Attempt3.V1.IEntityIdHeaderValueReader<TNextReader> entityIdHeaderValueReader)
                    {
                        // no reason to call `entityIdHeaderValueReader.V2Placeholder`; it doesn't go anywhere, and it's supposed to throw anyway

                        entityIdHeaderValueReader.TryMoveNext2(out var entityIdReader);

                        entityIdReader.TryGetValue2(out var entityId);

                        // process `entityid`

                        entityIdReader.TryMoveNext2(out var next);

                        return next;
                    }
                }
            }

            namespace V2IsReleasedByTheyAreStillOnlyUsingV1Features
            {
                public static class Play
                {
                    public static TNextReader Read<TNextReader>(Attempt3.V2.IEntityIdHeaderValueReader<TNextReader> entityIdHeaderValueReader)
                    {
                        // no reason to call `entityIdHeaderValueReader.V2Placeholder`; it doesn't go anywhere, and it's supposed to throw anyway

                        entityIdHeaderValueReader.TryMoveNext2(out var entityIdReader);

                        entityIdReader.TryGetValue2(out var entityId);

                        // process `entityid`

                        entityIdReader.TryMoveNext2(out var next);

                        return next;
                    }
                }
            }

            namespace V2IsReleasedAndTheyUseV2Features
            {
                public static class Play
                {
                    public static TNextReader Read<TNextReader>(Attempt3.V2.IEntityIdHeaderValueReader<TNextReader> entityIdHeaderValueReader)
                    {
                        // NOTE: the caller here has to know whether they are receiving something that supports v2...
                        // TODO what you could do is have a TVersion type parameter in all of the readers
                        entityIdHeaderValueReader.V2Placeholder.TryMoveNext2(out var entityIdStartReader);
                        entityIdStartReader.TryMoveNext2(out var iriSchemeReader);
                        iriSchemeReader.TryGetValue2(out var iriScheme);

                        // process irischeme

                        iriSchemeReader.TryMoveNext2(out var next);

                        return next;
                    }
                }
            }
        }

        namespace ReaderImplementer
        {
            namespace V1
            {
                public sealed class EntityIdHeaderValueReader<TNextReader> : Attempt3.V1.IEntityIdHeaderValueReader<TNextReader>
                {
                    private readonly Attempt3.V1.IEntityIdHeaderValueReader<TNextReader> delegateReader;

                    public EntityIdHeaderValueReader(Attempt3.V1.IEntityIdHeaderValueReader<TNextReader> delegateReader)
                    {
                        this.delegateReader = delegateReader;
                    }

                    public Attempt3.V1.IV2Placeholder<TNextReader> V2Placeholder => throw new NotImplementedException("TODO this exception is by design actually");

                    public async ValueTask Read()
                    {
                        await this.delegateReader.Read().ConfigureAwait(false);
                    }

                    public Attempt3.V1.IEntityIdReader<TNextReader> TryMoveNext(out bool moved)
                    {
                        var delegateEntityIdReader = this.delegateReader.TryMoveNext(out moved);
                        if (!moved)
                        {
                            return default;
                        }

                        return new EntityIdReader(delegateEntityIdReader);
                    }

                    private sealed class EntityIdReader : Attempt3.V1.IEntityIdReader<TNextReader>
                    {
                        private readonly Attempt3.V1.IEntityIdReader<TNextReader> delegateReader;

                        public EntityIdReader(Attempt3.V1.IEntityIdReader<TNextReader> delegateReader)
                        {
                            this.delegateReader = delegateReader;
                        }

                        public async ValueTask Read()
                        {
                            await this.delegateReader.Read().ConfigureAwait(false);
                        }

                        public EntityId TryGetValue(out bool moved)
                        {
                            var entityId = this.delegateReader.TryGetValue(out moved);
                            if (!moved)
                            {
                                return default;
                            }

                            // manipulate entityid to always use the "https://" scheme
                            return entityId;
                        }

                        public TNextReader TryMoveNext(out bool moved)
                        {
                            return this.delegateReader.TryMoveNext(out moved);
                        }
                    }
                }

                public sealed class V2Placeholder<TNextReader> : Attempt3.V1.IV2Placeholder<TNextReader>
                {
                    void Attempt3.V1.IV2Placeholder<TNextReader>.NoImplementation()
                    {
                        // NOTE: this can't actually be implemented in external projects; this is by design
                        throw new NotImplementedException();
                    }
                }
            }

            namespace V2IsReleasedButTheyHaventAddedAnyFeatures
            {
                public sealed class EntityIdHeaderValueReader<TNextReader> : Attempt3.V2.IEntityIdHeaderValueReader<TNextReader>
                {
                    private readonly Attempt3.V2.IEntityIdHeaderValueReader<TNextReader> delegateReader;

                    public EntityIdHeaderValueReader(Attempt3.V2.IEntityIdHeaderValueReader<TNextReader> delegateReader)
                    {
                        this.delegateReader = delegateReader;
                    }

                    public Attempt3.V2.IV2Placeholder<TNextReader> V2Placeholder => throw new NotImplementedException("TODO this exception is by design actually");

                    public async ValueTask Read()
                    {
                        await this.delegateReader.Read().ConfigureAwait(false);
                    }

                    public Attempt3.V2.IEntityIdReader<TNextReader> TryMoveNext(out bool moved)
                    {
                        var delegateEntityIdReader = this.delegateReader.TryMoveNext(out moved);
                        if (!moved)
                        {
                            return default;
                        }

                        return new EntityIdReader(delegateEntityIdReader);
                    }

                    private sealed class EntityIdReader : Attempt3.V2.IEntityIdReader<TNextReader>
                    {
                        private readonly Attempt3.V2.IEntityIdReader<TNextReader> delegateReader;

                        public EntityIdReader(Attempt3.V2.IEntityIdReader<TNextReader> delegateReader)
                        {
                            this.delegateReader = delegateReader;
                        }

                        public async ValueTask Read()
                        {
                            await this.delegateReader.Read().ConfigureAwait(false);
                        }

                        public EntityId TryGetValue(out bool moved)
                        {
                            var entityId = this.delegateReader.TryGetValue(out moved);
                            if (!moved)
                            {
                                return default;
                            }

                            // manipulate entityid to always use the "https://" scheme
                            return entityId;
                        }

                        public TNextReader TryMoveNext(out bool moved)
                        {
                            return this.delegateReader.TryMoveNext(out moved);
                        }
                    }
                }

                // NOTE: we didn't actually implement this in v1 either: public sealed class V2Placeholder<TNextReader> : Attempt3.V2.IV2Placeholder<TNextReader>
            }

            namespace V2IsReleasedAndLeveraged
            {
                public sealed class EntityIdHeaderValueReader<TNextReader> : Attempt3.V2.IEntityIdHeaderValueReader<TNextReader>
                {
                    private readonly Attempt3.V2.IEntityIdHeaderValueReader<TNextReader> delegateReader;

                    public EntityIdHeaderValueReader(Attempt3.V2.IEntityIdHeaderValueReader<TNextReader> delegateReader)
                    {
                        this.delegateReader = delegateReader;
                    }

                    public Attempt3.V2.IV2Placeholder<TNextReader> V2Placeholder
                    {
                        get
                        {
                            return new Placeholder(this.delegateReader);
                        }
                    }

                    private sealed class Placeholder : IV2Placeholder<TNextReader>
                    {
                        private readonly Attempt3.V2.IEntityIdHeaderValueReader<TNextReader> delegateReader;

                        public Placeholder(Attempt3.V2.IEntityIdHeaderValueReader<TNextReader> delegateReader)
                        {
                            this.delegateReader = delegateReader;
                        }

                        public async ValueTask Read()
                        {
                            await this.delegateReader.Read().ConfigureAwait(false);
                        }

                        public V2.IEntityIdStartReader<TNextReader> TryMoveNext(out bool moved)
                        {
                            var entityIdStartReader = this.delegateReader.V2Placeholder.TryMoveNext(out moved);
                            if (!moved)
                            {
                                return default;
                            }

                            return new EntityIdStartReader(entityIdStartReader);
                        }

                        private sealed class EntityIdStartReader : V2.IEntityIdStartReader<TNextReader>
                        {
                            private readonly Attempt3.V2.IEntityIdStartReader<TNextReader> entityIdStartReader;

                            public EntityIdStartReader(Attempt3.V2.IEntityIdStartReader<TNextReader> entityIdStartReader)
                            {
                                this.entityIdStartReader = entityIdStartReader;
                            }

                            public async ValueTask Read()
                            {
                                await this.entityIdStartReader.Read().ConfigureAwait(false);
                            }

                            public V2.IIriSchemeReader<TNextReader> TryMoveNext(out bool moved)
                            {
                                var iriSchemeReader = this.entityIdStartReader.TryMoveNext(out moved);
                                if (!moved)
                                {
                                    return default;
                                }

                                return new IriSchemeReader(iriSchemeReader);
                            }

                            private sealed class IriSchemeReader : V2.IIriSchemeReader<TNextReader>
                            {
                                private readonly V2.IIriSchemeReader<TNextReader> iriSchemeReader;

                                public IriSchemeReader(V2.IIriSchemeReader<TNextReader> iriSchemeReader)
                                {
                                    this.iriSchemeReader = iriSchemeReader;
                                }

                                public async ValueTask Read()
                                {
                                    await this.iriSchemeReader.Read().ConfigureAwait(false);
                                }

                                public IriScheme TryGetValue(out bool moved)
                                {
                                    var iriScheme = this.iriSchemeReader.TryGetValue(out moved);
                                    if (!moved)
                                    {
                                        return default;
                                    }

                                    // manipulate `irischeme` to be "https://"
                                    return iriScheme;
                                }

                                public TNextReader TryMoveNext(out bool moved)
                                {
                                    return this.iriSchemeReader.TryMoveNext(out moved);
                                }
                            }
                        }
                    }

                    public async ValueTask Read()
                    {
                        await this.ToV1().Read().ConfigureAwait(false);
                    }

                    public Attempt3.V2.IEntityIdReader<TNextReader> TryMoveNext(out bool moved)
                    {
                        return this.ToV1().TryMoveNext(out moved);
                    }
                }

                // NOTE: we didn't actually implement this in v1 either: public sealed class V2Placeholder<TNextReader> : Attempt3.V2.IV2Placeholder<TNextReader>
            }
        }


        //// TODO 3 personas: odata maintainer, reader implementer, reader caller
    }
}
