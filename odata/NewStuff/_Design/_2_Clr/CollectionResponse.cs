namespace NewStuff._Design._2_Clr
{
    using System;
    using System.Collections.Generic;

    public class CollectionResponse<TEntity, TKey>
    {
        public CollectionResponse(IEnumerable<CollectionResponseEntity<TEntity, TKey>> values, Uri? nextLink, long? count, string? type)
        {
            Values = values;
            NextLink = nextLink;
            Count = count;
            Type = type;
        }

        public IEnumerable<CollectionResponseEntity<TEntity, TKey>> Values { get; }

        public Uri? NextLink { get; } //// TODO the type of this property should be stronger

        public long? Count { get; }

        public string? Type { get; } //// TODO this should be more strongly-typed
    }
}
