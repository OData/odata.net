namespace NewStuff._Design._1_Protocol
{
    using System.Collections.Generic;

    public class MultiValuedRequestProperty
    {
        public MultiValuedRequestProperty(string name, IEnumerable<SingleValuedRequest> values)
        {
            Name = name;
            Values = values;
        }

        public string Name { get; }

        public IEnumerable<SingleValuedRequest> Values { get; } //// TODO i don't think you should be using the same type for a "root" single-value *and* a "nested" single-value
    }
}
