namespace NewStuff._Design._2_Clr
{
    using System;

    public class ResponseProperty<T>
    {
        public string? Type { get; } //// TODO this should be more strongly-typed
    }

    public class ResponseCollectionProperty<T> : ResponseProperty<T>
    {
        public Uri? NextLink { get; } //// TODO the type of this property should be stronger

        public long? Count { get; }

        public string? SkipToken { get; } //// TODO this should probably be more strongly typed
    }

    public class ResponseSingleValuedProperty<T> : ResponseProperty<T>
    {
        public string? Id { get; } //// TODO this should be more strongly-typed
    }
}
