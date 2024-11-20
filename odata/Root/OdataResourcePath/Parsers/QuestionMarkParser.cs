namespace Root.OdataResourcePath.Parsers
{
    using Root.OdataResourcePath.ConcreteSyntaxTreeNodes;
    using Sprache;

    public static class QuestionMarkParser
    {
        public static Parser<QuestionMark> Instance { get; } = Parse
            .String("?")
            .Return(QuestionMark.Instance);
    }
}
