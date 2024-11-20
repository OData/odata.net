namespace Root.OdataResourcePath.Parsers
{
    using Root.OdataResourcePath.ConcreteSyntaxTreeNodes;
    using Sprache;

    public static class EntityCastOptionsParser
    {
        public static Parser<EntityCastOptions> Instance { get; } = Parser.None<EntityCastOptions>(
            "BatchOptions parsing has not been implemented");
    }
}
