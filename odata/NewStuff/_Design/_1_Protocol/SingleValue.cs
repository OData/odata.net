namespace NewStuff._Design._1_Protocol
{
    using System.Collections.Generic;

    public class SingleValue
    {
        public SingleValue(IEnumerable<ComplexResponseProperty> complexProperties, IEnumerable<MultiValuedResponseProperty> multiValuedProperties, IEnumerable<UntypedResponseProperty> untypedProperties, IEnumerable<PrimitiveResponseProperty> primitiveProperties, IEnumerable<DynamicResponseProperty> dynamicProperties, string? context)
        {
            ComplexProperties = complexProperties;
            MultiValuedProperties = multiValuedProperties;
            UntypedProperties = untypedProperties;
            PrimitiveProperties = primitiveProperties;
            DynamicProperties = dynamicProperties;
            Context = context;
        }

        //// TODO you need to model all the dimensions of single vs multi + complex vs primitive vs untyped vs dynamic

        public IEnumerable<ComplexResponseProperty> ComplexProperties { get; }

        public IEnumerable<MultiValuedResponseProperty> MultiValuedProperties { get; }

        public IEnumerable<UntypedResponseProperty> UntypedProperties { get; }

        public IEnumerable<PrimitiveResponseProperty> PrimitiveProperties { get; }

        public IEnumerable<DynamicResponseProperty> DynamicProperties { get; }

        public string? Context { get; } //// TODO make this strongly typed
    }
}
