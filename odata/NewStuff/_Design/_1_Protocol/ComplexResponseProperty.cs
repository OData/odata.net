namespace NewStuff._Design._1_Protocol
{
    using System.Collections.Generic;

    public class ComplexResponseProperty
    {
        public string Name { get; }

        public SingleValue Value { get; }

        public IEnumerable<object> Annotations { get; } //// TODO make this strongly typed
    }
}
