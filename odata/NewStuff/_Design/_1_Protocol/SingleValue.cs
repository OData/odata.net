namespace NewStuff._Design._1_Protocol
{
    using System.Collections.Generic;

    public class SingleValue
    {
        //// TODO you need to model all the dimensions of single vs multi + complex vs primitive vs untyped vs dynamic

        public IEnumerable<ComplexResponseProperty> ComplexProperties { get; }

        public IEnumerable<MultiValuedResponseProperty> MultiValuedProperties { get; }

        public IEnumerable<UntypedResponseProperty> UntypedProperties { get; }

        public IEnumerable<PrimitiveResponseProperty> PrimitiveProperties { get; }

        public IEnumerable<DynamicResponseProperty> DynamicProperties { get; }

        public string? Context { get; } //// TODO make this strongly typed
    }
}
