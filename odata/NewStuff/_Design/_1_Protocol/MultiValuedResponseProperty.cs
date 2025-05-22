namespace NewStuff._Design._1_Protocol
{
    using System.Collections.Generic;

    public class MultiValuedResponseProperty
    {
        public string Name { get; }

        public IEnumerable<SingleValue> Values { get; }

        public string? NextLink { get; } //// TODO more strongly typed?
    }
}
