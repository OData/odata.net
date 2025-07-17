namespace NewStuff._Design._0_Convention.V2
{
    public sealed class OdataContextInsideOfAComplexElementWithinAMultivaluedPropertyAtTheRootOfTheResponseBody
    {
        internal OdataContextInsideOfAComplexElementWithinAMultivaluedPropertyAtTheRootOfTheResponseBody(string context)
        {
            Context = context;
        }

        internal string Context { get; }
    }
}
