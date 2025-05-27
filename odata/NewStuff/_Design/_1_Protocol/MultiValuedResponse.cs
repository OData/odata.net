namespace NewStuff._Design._1_Protocol
{
    using System;
    using System.Collections.Generic;

    public class MultiValuedResponse
    {
        public MultiValuedResponse(IReadOnlyList<SingleValue> value, Uri? nextLink, int? count, IEnumerable<object> annotations)
        {
            Value = value;
            NextLink = nextLink;
            Count = count;
            Annotations = annotations;
        }

        //// TODO mimicing the modeling of JSON is probably a good idea

        public IReadOnlyList<SingleValue> Value { get; }

        public Uri? NextLink { get; } //// TODO make this strongly-typed

        public int? Count { get; } //// TODO does this need to be strongly-typed? (maybe to avoid nullable?)

        public IEnumerable<object> Annotations { get; } //// TODO make this strongly-typed
    }
}
