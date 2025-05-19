namespace NewStuff._Design._2_Clr
{
    using System;
    using System.Collections.Generic;

    public class CollectionResponse<TEntity>
    {
        public IEnumerable<ResponseEntity<TEntity>> Values { get; }

        public Uri? NextLink { get; } //// TODO the type of this property should be stronger

        public long? Count { get; }

        public string? Type { get; } //// TODO this should be more strongly-typed

        public string? SkipToken { get; } //// TODO this should probably be more strongly typed
    }
}
