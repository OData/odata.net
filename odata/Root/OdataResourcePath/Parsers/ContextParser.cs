namespace Root.OdataResourcePath.Parsers
{
    using Root.OdataResourcePath.ConcreteSyntaxTreeNodes;
    using Sprache;

    public static class ContextParser
    {
        public static Parser<Context> Instance { get; } = Parser.None<Context>(
            "BatchOptions parsing has not been implemented");
    }
}
