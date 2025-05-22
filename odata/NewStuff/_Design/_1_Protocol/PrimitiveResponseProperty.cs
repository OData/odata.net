namespace NewStuff._Design._1_Protocol
{
    using System.Collections.Generic;

    public class PrimitiveResponseProperty
    {
        public string Name { get; }

        public string? Value { get; } //// TODO strongly type this? how to handle nulls?

        public IEnumerable<object> Annotations { get; } //// TODO strongly-type this
    }
}
