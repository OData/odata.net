namespace Root.OdataResourcePath.CombinatorParsers
{
    using Root.OdataResourcePath.ConcreteSyntaxTreeNodes;
    using Sprache;

    public static class SlashParser
    {
        public static Parser<Slash> Instance { get; } = Parse
            .String("/")
            .Return(Slash.Instance);
    }
}
