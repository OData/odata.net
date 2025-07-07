namespace NewStuff._Design._0_Convention.V2
{
    public sealed class OdataContext
    {
        internal OdataContext(string context)
        {
            Context = context;
        }

        internal string Context { get; }
    }
}
