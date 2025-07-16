namespace NewStuff._Design._0_Convention.V2
{
    public sealed class RootComplexElementPrimitivePropertyValue
    {
        internal RootComplexElementPrimitivePropertyValue(string value)
        {
            Value = value;
        }

        internal string Value { get; }

        //// TODO this should probably be a "token" and have different members for each kind of primitive property; or maybe you can't do that without the edm model so this isn't the right place?
    }
}
