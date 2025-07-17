namespace NewStuff._Design._0_Convention.V2
{
    public sealed class OdataContextAtTheRootOfTheResponseBody
    {
        internal OdataContextAtTheRootOfTheResponseBody(string context)
        {
            Context = context;
        }

        internal string Context { get; }
    }
}
