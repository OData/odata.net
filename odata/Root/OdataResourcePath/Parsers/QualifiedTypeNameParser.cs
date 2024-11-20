namespace Root.OdataResourcePath.Parsers
{
    using Root.OdataResourcePath.ConcreteSyntaxTreeNodes;
    using Sprache;

    public static class QualifiedTypeNameParser
    {
        //// TODO how to implement this for the "Feature gap" case?
        public static Parser<QualifiedEntityTypeName> Instance { get; }
    }
}
