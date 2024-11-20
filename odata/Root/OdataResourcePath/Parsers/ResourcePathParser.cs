namespace Root.OdataResourcePath.Parsers
{
    using Root.OdataResourcePath.ConcreteSyntaxTreeNodes;
    using Sprache;

    public static class ResourcePathParser
    {
        public static Parser<ResourcePath> Instance { get; } = Parser.None<ResourcePath>(
            "BatchOptions parsing has not been implemented");
    }
}
