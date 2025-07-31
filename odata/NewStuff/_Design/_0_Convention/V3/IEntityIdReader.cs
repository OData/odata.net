namespace NewStuff._Design._0_Convention.V3
{
    public interface IEntityIdReader<out TNextReader> : IReader<EntityId, TNextReader>
    {
    }

    public interface IEntityIdStartReader<out TNextReader> : IReader<IIriSchemeReader<TNextReader>>
    {
    }

    public interface IIriSchemeReader<out TNextReader>
    {
    }

    //// TODO you are in 4.1 of this doc: https://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html
    //// you are trying to decide if you should go ahead and dive into iri parsing (and if you don't, what will it look like when you decide that you want to, because up to this point, you would have called the entry point reader for the "broken down" version `ientityidreader`, but then that would conflict with the current name); and also, how it's going to look to write an `entityid` when you can only write uris and not iris; basically, what should you do when reading and writing are not symmetric

    public sealed class EntityId
    {
        private EntityId()
        {
        }
    }
}
