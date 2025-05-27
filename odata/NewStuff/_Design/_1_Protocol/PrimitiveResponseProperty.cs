namespace NewStuff._Design._1_Protocol
{
    using System.Collections.Generic;

    public class PrimitiveResponseProperty
    {
        public PrimitiveResponseProperty(string name, string? value, IEnumerable<object> annotations)
        {
            Name = name;
            Value = value;
            Annotations = annotations;
        }

        public string Name { get; }

        public string? Value { get; } //// TODO strongly type this? how to handle nulls?

        public IEnumerable<object> Annotations { get; } //// TODO strongly-type this
    }
}
