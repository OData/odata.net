namespace Root.OdataResourcePath.Parsers
{
    using Root.OdataResourcePath.ConcreteSyntaxTreeNodes;
    using Sprache;

    public static class EntityParser
    {
        public static Parser<Entity> Instance { get; } = Parse
            .String("$entity")
            .Return(Entity.Instance);
    }
}
