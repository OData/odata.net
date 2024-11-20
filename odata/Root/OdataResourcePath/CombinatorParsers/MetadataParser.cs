namespace Root.OdataResourcePath.CombinatorParsers
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
