namespace NewStuff._Design._0_Convention.V2
{
    public sealed class RootComplexElementOdataContext
    {
        internal RootComplexElementOdataContext(string context)
        {
            Context = context;
        }

        internal string Context { get; }
    }
}
