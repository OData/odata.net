namespace NewStuff._Design._1_Protocol
{
    using System.Collections.Generic;

    public class SingleValuedRequest
    {
        public SingleValuedRequest(IEnumerable<ComplexRequestProperty> complexProperties, IEnumerable<MultiValuedRequestProperty> multiValuedProperties, IEnumerable<UntypedRequestProperty> untypedProperties, IEnumerable<PrimitiveRequestProperty> primitiveProperties, IEnumerable<DynamicRequestProperty> dynamicProperties)
        {
            ComplexProperties = complexProperties;
            MultiValuedProperties = multiValuedProperties;
            UntypedProperties = untypedProperties;
            PrimitiveProperties = primitiveProperties;
            DynamicProperties = dynamicProperties;
        }

        public IEnumerable<ComplexRequestProperty> ComplexProperties { get; }

        public IEnumerable<MultiValuedRequestProperty> MultiValuedProperties { get; }

        public IEnumerable<UntypedRequestProperty> UntypedProperties { get; }

        public IEnumerable<PrimitiveRequestProperty> PrimitiveProperties { get; }

        public IEnumerable<DynamicRequestProperty> DynamicProperties { get; }
    }
}
