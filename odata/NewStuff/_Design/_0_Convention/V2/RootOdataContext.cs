namespace NewStuff._Design._0_Convention.V2
{
    public sealed class RootOdataContext
    {
        internal RootOdataContext(string context)
        {
            Context = context;
        }

        internal string Context { get; }
    }
}
