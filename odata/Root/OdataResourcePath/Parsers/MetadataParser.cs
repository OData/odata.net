namespace Root.OdataResourcePath.Parsers
{
    using Root.OdataResourcePath.ConcreteSyntaxTreeNodes;
    using Sprache;

    public static class MetadataParser
    {
        public static Parser<Metadata> Instance { get; } = Parse
            .String("$metadata")
            .Return(Metadata.Instance);
    }
}
