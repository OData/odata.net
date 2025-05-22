namespace NewStuff._Design._1_Protocol
{
    using System.Collections.Generic;

    public class SingleValuedResponse
    {
        //// TODO mimicing the modeling of JSON is probably a good idea

        public SingleValue? Value { get; } //// TODO you don't intend this to be "nullable", you intend to indicate if the value was provided

        public IEnumerable<object> Annotations { get; } //// TODO make this strongly-typed
    }
}
