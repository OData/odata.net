using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

using NewStuff._Design._0_Convention.V3.Attempt2.V2;
using NewStuff._Design._0_Convention.V3.Attempt3.V1;
using NewStuff._Design._0_Convention.V3.Attempt3.V2;

namespace NewStuff._Design._0_Convention.V3
{
    //// TODO you are in 4.1 of this doc: https://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html
    //// you are trying to decide if you should go ahead and dive into iri parsing (and if you don't, what will it look like when you decide that you want to, because up to this point, you would have called the entry point reader for the "broken down" version `ientityidreader`, but then that would conflict with the current name); and also, how it's going to look to write an `entityid` when you can only write uris and not iris; basically, what should you do when reading and writing are not symmetric

    //// TODO i think the `attempt2` namespace is the right track; finish the extension method, then implement some readers, and then write some fake calling code




    public static class ForClement
    {
        public class Customer
        {
            public string Name { get; }
        }

        public class GetNextLinkImplementation<TState>
        {
            private readonly object globalState;

            public GetNextLinkImplementation(object globalState)
            {
                this.globalState = globalState;
            }

            public string GetNextLink(Customer customer, TState state)
            {
                return customer.Name;
            }
        }

        public static void Play()
        {
            var typeInfo = new OdataTypeInfo<Customer, bool>()
            {
                Properties = new List<OdataPropertyInfo<Customer, bool>>()
                {
                    new OdataPropertyInfo<Customer, bool, string>()
                    {
                        GetNextLink = new GetNextLinkImplementation<bool>(new object()).GetNextLink,
                    }
                },
            };
        }

        public class OdataTypeInfo<T1, T2>
        {
            public List<OdataPropertyInfo<T1, T2>> Properties { get; set; }
        }

        public class OdataPropertyInfo<T1, T2>
        {
        }

        public class OdataPropertyInfo<T1, T2, T3> : OdataPropertyInfo<T1, T2>
        {
            public Func<T1, T2, T3> GetNextLink { get; set; }
        }
    }


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

    public interface IEntityIdHeaderValueReaderV1<out TNextReader> : IReader<IEntityIdReader<TNextReader>>
    {
    }

    public interface IEntityIdHeaderValueReaderV2<out TNextReader> : IReader<IEntityIdReader<TNextReader>>
    {
        IEntityIdStartReader<TNextReader> Next { get; }
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

                    private sealed class Impl : V1
                    {
                        internal override void NoImplementation()
                        {
                            throw new NotImplementedException();
                        }
                    }

                    public static V1 Instance { get; } = new Impl();
                }
            }

            public interface IEntityIdHeaderValueReader<out TVersion, out TNextReader> : IReader<IEntityIdReader<TVersion, TNextReader>>
                where TVersion : Version.V1
            {
                TVersion Version { get; }
            }

            public interface IEntityIdReader<out TVersion, out TNextReader> : IReader<EntityId, TNextReader>
                where TVersion : Version.V1
            {
                TVersion Version { get; }
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
                    private readonly IV2Implementations v2Implementations;

                    private V2(IV2Implementations v2Implementations)
                    {
                        this.v2Implementations = v2Implementations;
                    }

                    public static V2 Create(IV2Implementations v2Implementations)
                    {
                        return new Instance(v2Implementations);
                    }

                    private sealed class Instance : V2
                    {
                        public Instance(IV2Implementations v2Implementations)
                            : base(v2Implementations)
                        {
                        }

                        internal override void NoImplementation()
                        {
                            throw new NotImplementedException();
                        }
                    }

                    internal abstract void NoImplementation();

                    public bool TryMoveNext<TVersion, TNextReader>(IEntityIdHeaderValueReader<TVersion, TNextReader> entityIdHeaderValueReader, out IEntityIdStartReader<TVersion, TNextReader> entityIdStartReader)
                        where TVersion : Version.V2
                    {
                        return this.v2Implementations.TryMoveNext(entityIdHeaderValueReader, out entityIdStartReader);
                    }
                }

                public interface IV2Implementations //// TODO newer versions shold probably implement the previous versions so that the reader implementer doesn't have to chain those together themselves
                {
                    bool TryMoveNext<TVersion, TNextReader>(IEntityIdHeaderValueReader<TVersion, TNextReader> entityIdHeaderValueReader, out IEntityIdStartReader<TVersion, TNextReader> entityIdStartReader)
                        where TVersion : Version.V2;
                }
            }

            //// TODO you are here trying to do the version thing; make sure that ireader<v2> can be cast to ireader<v1>; you also thought about exploring if the `v1`, `v2`, etc. classes could have the "get extended reader" methods, so for example, ientityidheadervaluereader would have a `TVersion Version { get; }` and then this could be called to say `reader.Version.GetExtended(entityIdHeaderValueReader)` and it'd give you back the irireader instead of the entityidreader

            public interface IEntityIdHeaderValueReader<out TVersion, out TNextReader> : IReader<IEntityIdReader<TVersion, TNextReader>>
                where TVersion : Version
            {
                TVersion Version { get; }
            }

            public interface IEntityIdReader<out TVersion, out TNextReader> : IReader<EntityId, TNextReader>
                where TVersion : Version
            {
                TVersion Version { get; }
            }

            public interface IEntityIdStartReader<out TVersion, out TNextReader> : IReader<IIriSchemeReader<TVersion, TNextReader>>
                where TVersion : Version
            {
                TVersion Version { get; }
            }

            public interface IIriSchemeReader<out TVersion, out TNextReader> : IReader<IriScheme, TNextReader>
                where TVersion : Version
            {
                TVersion Version { get; }
            }

            public struct IriScheme
            {
            }

            public static class Extensions
            {
                public static IEntityIdHeaderValueReader<TVersion, TNextReader> ToV1<TVersion, TNextReader>(this IEntityIdHeaderValueReader<TVersion, TNextReader> entityIdHeaderValueReader)
                    where TVersion : Version.V2
                {
                    entityIdHeaderValueReader.Version.TryMoveNext(entityIdHeaderValueReader, out var entityIdStartReader);

                    return new EntityIdHeaderValueReader<TVersion, TNextReader>(entityIdStartReader);
                }

                private sealed class EntityIdHeaderValueReader<TVersion, TNextReader> : IEntityIdHeaderValueReader<TVersion, TNextReader>
                    where TVersion : Version.V2
                {
                    private readonly IEntityIdStartReader<TVersion, TNextReader> v2Placeholder;

                    public EntityIdHeaderValueReader(IEntityIdStartReader<TVersion, TNextReader> v2Placeholder)
                    {
                        this.v2Placeholder = v2Placeholder;
                    }

                    public IEntityIdStartReader<TVersion, TNextReader> EntityIdStartReader
                    {
                        get
                        {
                            return this.v2Placeholder;
                        }
                    }

                    public TVersion Version
                    {
                        get
                        {
                            return Attempt3.V2.Version.V2.Create(new Implementations());
                        }
                    }

                    private sealed class Implementations : Attempt3.V2.Version.V2.IV2Implementations
                    {


                        public bool TryMoveNext<TVersion1, TNextReader1>(IEntityIdHeaderValueReader<TVersion1, TNextReader1> entityIdHeaderValueReader, out IEntityIdStartReader<TVersion1, TNextReader1> entityIdStartReader) where TVersion1 : Version.V2
                        {
                            entityIdStartReader = new EntityIdReader();
                            return true;
                        }
                    }

                    public ValueTask Read()
                    {
                        return ValueTask.CompletedTask;
                    }

                    public IEntityIdReader<TVersion, TNextReader> TryMoveNext(out bool moved)
                    {
                        moved = true;
                        return new EntityIdReader(this.v2Placeholder);
                    }

                    private sealed class EntityIdReader : IEntityIdReader<TVersion, TNextReader>
                    {
                        private readonly IEntityIdStartReader<TVersion, TNextReader> v2Placeholder;

                        private IIriSchemeReader<TVersion, TNextReader>? iriSchemeReader;

                        public EntityIdReader(IEntityIdStartReader<TVersion, TNextReader> v2Placeholder)
                        {
                            this.v2Placeholder = v2Placeholder;
                        }

                        public async ValueTask Read()
                        {
                            if (this.iriSchemeReader != null)
                            {
                                await this.iriSchemeReader.Read().ConfigureAwait(false);
                            }
                            else
                            {
                                await this.v2Placeholder.Read().ConfigureAwait(false);
                            }
                        }

                        public EntityId TryGetValue(out bool moved)
                        {
                            if (this.iriSchemeReader == null)
                            {
                                this.iriSchemeReader = this.v2Placeholder.TryMoveNext(out moved);
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
                    public static TNextReader Read<TNextReader>(Attempt3.V1.IEntityIdHeaderValueReader<V1.Version.V1, TNextReader> entityIdHeaderValueReader)
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
                    public static TNextReader Read<TNextReader>(Attempt3.V2.IEntityIdHeaderValueReader<V2.Version.V1, TNextReader> entityIdHeaderValueReader)
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
                    public static TNextReader Read<TNextReader>(Attempt3.V2.IEntityIdHeaderValueReader<V2.Version.V2,    TNextReader> entityIdHeaderValueReader)
                    {
                        // NOTE: the caller here has to know whether they are receiving something that supports v2...
                        // TODO what you could do is have a TVersion type parameter in all of the readers
                        entityIdHeaderValueReader.Version.TryMoveNext(entityIdHeaderValueReader, out var entityIdStartReader);
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
                public sealed class EntityIdHeaderValueReader<TNextReader> : Attempt3.V1.IEntityIdHeaderValueReader<Attempt3.V1.Version.V1, TNextReader>
                {
                    private readonly Attempt3.V1.IEntityIdHeaderValueReader<Attempt3.V1.Version.V1, TNextReader> delegateReader;

                    public EntityIdHeaderValueReader(Attempt3.V1.IEntityIdHeaderValueReader<Attempt3.V1.Version.V1, TNextReader> delegateReader)
                    {
                        this.delegateReader = delegateReader;
                    }

                    public Attempt3.V1.IEntityIdStartReader<Attempt3.V1.Version.V1, TNextReader> EntityIdStartReader => throw new NotImplementedException("TODO this exception is by design actually");

                    public async ValueTask Read()
                    {
                        await this.delegateReader.Read().ConfigureAwait(false);
                    }

                    public Attempt3.V1.IEntityIdReader<Attempt3.V1.Version.V1, TNextReader> TryMoveNext(out bool moved)
                    {
                        var delegateEntityIdReader = this.delegateReader.TryMoveNext(out moved);
                        if (!moved)
                        {
                            return default;
                        }

                        return new EntityIdReader(delegateEntityIdReader);
                    }

                    private sealed class EntityIdReader : Attempt3.V1.IEntityIdReader<Attempt3.V1.Version.V1, TNextReader>
                    {
                        private readonly Attempt3.V1.IEntityIdReader<Attempt3.V1.Version.V1, TNextReader> delegateReader;

                        public EntityIdReader(Attempt3.V1.IEntityIdReader<Attempt3.V1.Version.V1, TNextReader> delegateReader)
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

                public sealed class V2Placeholder<TNextReader> : Attempt3.V1.IEntityIdStartReader<Attempt3.V1.Version.V1, TNextReader>
                {
                    void Attempt3.V1.IEntityIdStartReader<Attempt3.V1.Version.V1, TNextReader>.NoImplementation()
                    {
                        // NOTE: this can't actually be implemented in external projects; this is by design
                        throw new NotImplementedException();
                    }
                }
            }

            namespace V2IsReleasedButTheyHaventAddedAnyFeatures
            {
                public sealed class EntityIdHeaderValueReader<TNextReader> : Attempt3.V2.IEntityIdHeaderValueReader<Attempt3.V2.Version.V1, TNextReader>
                {
                    private readonly Attempt3.V2.IEntityIdHeaderValueReader<Attempt3.V2.Version.V1, TNextReader> delegateReader;

                    public EntityIdHeaderValueReader(Attempt3.V2.IEntityIdHeaderValueReader<Attempt3.V2.Version.V1, TNextReader> delegateReader)
                    {
                        this.delegateReader = delegateReader;
                    }

                    public Attempt3.V2.IEntityIdStartReader<Attempt3.V2.Version.V1, TNextReader> EntityIdStartReader => throw new NotImplementedException("TODO this exception is by design actually");

                    public async ValueTask Read()
                    {
                        await this.delegateReader.Read().ConfigureAwait(false);
                    }

                    public Attempt3.V2.IEntityIdReader<Attempt3.V2.Version.V1, TNextReader> TryMoveNext(out bool moved)
                    {
                        var delegateEntityIdReader = this.delegateReader.TryMoveNext(out moved);
                        if (!moved)
                        {
                            return default;
                        }

                        return new EntityIdReader(delegateEntityIdReader);
                    }

                    private sealed class EntityIdReader : Attempt3.V2.IEntityIdReader<Attempt3.V2.Version.V1, TNextReader>
                    {
                        private readonly Attempt3.V2.IEntityIdReader<Attempt3.V2.Version.V1, TNextReader> delegateReader;

                        public EntityIdReader(Attempt3.V2.IEntityIdReader<Attempt3.V2.Version.V1, TNextReader> delegateReader)
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
                public sealed class EntityIdHeaderValueReader<TNextReader> : Attempt3.V2.IEntityIdHeaderValueReader<Attempt3.V2.Version.V2, TNextReader>
                {
                    private readonly Attempt3.V2.IEntityIdHeaderValueReader<Attempt3.V2.Version.V2, TNextReader> delegateReader;

                    public EntityIdHeaderValueReader(Attempt3.V2.IEntityIdHeaderValueReader<Attempt3.V2.Version.V2, TNextReader> delegateReader)
                    {
                        this.delegateReader = delegateReader;
                    }

                    public Attempt3.V2.IEntityIdStartReader<Attempt3.V2.Version.V2, TNextReader> EntityIdStartReader
                    {
                        get
                        {
                            return new Placeholder(this.delegateReader);
                        }
                    }

                    private sealed class Placeholder : IEntityIdStartReader<Attempt3.V2.Version.V2, TNextReader>
                    {
                        private readonly Attempt3.V2.IEntityIdHeaderValueReader<Attempt3.V2.Version.V2, TNextReader> delegateReader;

                        public Placeholder(Attempt3.V2.IEntityIdHeaderValueReader<Attempt3.V2.Version.V2, TNextReader> delegateReader)
                        {
                            this.delegateReader = delegateReader;
                        }

                        public async ValueTask Read()
                        {
                            await this.delegateReader.Read().ConfigureAwait(false);
                        }

                        public V2.IIriSchemeReader<Attempt3.V2.Version.V2, TNextReader> TryMoveNext(out bool moved)
                        {
                            var iriSchemeReader = this.delegateReader.EntityIdStartReader.TryMoveNext(out moved);
                            if (!moved)
                            {
                                return default;
                            }

                            return new IriSchemeReader(iriSchemeReader);
                        }

                        private sealed class IriSchemeReader : V2.IIriSchemeReader<Attempt3.V2.Version.V2, TNextReader>
                        {
                            private readonly V2.IIriSchemeReader<Attempt3.V2.Version.V2, TNextReader> iriSchemeReader;

                            public IriSchemeReader(V2.IIriSchemeReader<Attempt3.V2.Version.V2, TNextReader> iriSchemeReader)
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

                    public async ValueTask Read()
                    {
                        await this.ToV1().Read().ConfigureAwait(false);
                    }

                    public Attempt3.V2.IEntityIdReader<Attempt3.V2.Version.V2, TNextReader> TryMoveNext(out bool moved)
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
