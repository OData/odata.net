namespace Root.OdataResourcePath.Parsers
{
    using Root.OdataResourcePath.ConcreteSyntaxTreeNodes;
    using Sprache;

    public static class BatchOptionsParser
    {
        public static Parser<BatchOptions> Instance { get; } = Parser.None<BatchOptions>(
            "BatchOptions parsing has not been implemented");
    }
}
