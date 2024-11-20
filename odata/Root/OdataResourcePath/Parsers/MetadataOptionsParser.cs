namespace Root.OdataResourcePath.Parsers
{
    using Root.OdataResourcePath.ConcreteSyntaxTreeNodes;
    using Sprache;

    public static class MetadataOptionsParser
    {
        public static Parser<MetadataOptions> Instance { get; } = Parser.None<MetadataOptions>(
            "BatchOptions parsing has not been implemented");
    }
}
