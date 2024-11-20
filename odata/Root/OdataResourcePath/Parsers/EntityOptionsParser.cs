namespace Root.OdataResourcePath.Parsers
{
    using Root.OdataResourcePath.ConcreteSyntaxTreeNodes;
    using Sprache;

    public static class EntityOptionsParser
    {
        public static Parser<EntityOptions> Instance { get; } = Parser.None<EntityOptions>(
            "BatchOptions parsing has not been implemented");
    }
}
