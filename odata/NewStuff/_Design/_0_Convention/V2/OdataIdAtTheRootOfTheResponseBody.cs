namespace NewStuff._Design._0_Convention.V2
{
    public sealed class OdataIdAtTheRootOfTheResponseBody
    {
        internal OdataIdAtTheRootOfTheResponseBody(string value)
        {
            Value = value;
        }

        internal string Value { get; }
    }
}
