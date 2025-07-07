namespace NewStuff._Design._0_Convention.V2
{
    public sealed class ContentType
    {
        internal ContentType(string value)
        {
            Value = value;
        }

        internal string Value { get; } //// TODO more strongly-type this
    }
}
