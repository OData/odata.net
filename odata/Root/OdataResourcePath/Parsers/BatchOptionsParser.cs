namespace Root.OdataResourcePath.Parsers
{
    using Root.OdataResourcePath.ConcreteSyntaxTreeNodes;
    using Sprache;

    public static class BatchOptionsParser
    {
        //// TODO how to implement this for the "Feature gap" case?
        public static Parser<BatchOptions> Instance { get; }
    }
}
