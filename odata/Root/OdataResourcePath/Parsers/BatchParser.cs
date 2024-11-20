namespace Root.OdataResourcePath.Parsers
{
    using Root.OdataResourcePath.ConcreteSyntaxTreeNodes;
    using Sprache;

    public static class BatchParser
    {
        public static Parser<Batch> Instance { get; } = Parse
            .String("$batch")
            .Return(Batch.Instance);
    }
}
