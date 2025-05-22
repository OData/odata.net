namespace NewStuff._Design._1_Protocol
{
    public class PrimitiveRequestProperty
    {
        public PrimitiveRequestProperty(string name, string? value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }

        public string? Value { get; } //// TODO strongly type this? how to handle nulls?
    }
}
