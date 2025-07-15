namespace NewStuff._Design._0_Convention.V2
{
    public sealed class OdataId
    {
        internal OdataId(string value)
        {
            Value = value;
        }

        internal string Value { get; }
    }
}
