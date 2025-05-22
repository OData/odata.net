namespace NewStuff._Design._1_Protocol
{
    using System.Collections.Generic;

    public class MultiValuedResponse
    {
        public IReadOnlyList<SingleValue> Value { get; }

        public string? NextLink { get; } //// TODO make this strongly-typed

        public int? Count { get; } //// TODO does this need to be strongly-typed? (maybe to avoid nullable?)
    }
}
