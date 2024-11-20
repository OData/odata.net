namespace Root.OdataResourcePath.Parsers
{
    using Root.OdataResourcePath.ConcreteSyntaxTreeNodes;
    using Sprache;

    public static class EntityOptionsParser
    {
        //// TODO how to implement this for the "Feature gap" case?
        public static Parser<EntityOptions> Instance { get; }
    }
}
