namespace Root.OdataResourcePath.Parsers
{
    using Root.OdataResourcePath.ConcreteSyntaxTreeNodes;
    using Sprache;

    public static class QueryOptionsParser
    {
        //// TODO how to implement this for the "Feature gap" case?
        public static Parser<QueryOptions> Instance { get; } = Parser.None<QueryOptions>(
            "BatchOptions parsing has not been implemented");
    }
}
