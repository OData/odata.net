namespace NewStuff._Design._0_Convention.V2
{
    public sealed class RootOdataId
    {
        internal RootOdataId(string value)
        {
            Value = value;
        }

        internal string Value { get; }
    }
}
