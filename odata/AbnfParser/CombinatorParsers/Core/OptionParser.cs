namespace AbnfParser.CombinatorParsers.Core
{
    using AbnfParser.CstNodes;
    using Sprache;

    public static class OptionParser
    {
        public static Parser<Option> Instance { get; } =
            from openBracket in x5BParser.Instance
            from prefixCwsps in Parse.Many(CwspParser.Instance)
            from alternation in AlternationParser.Instance
            from suffixCwsps in Parse.Many(CwspParser.Instance)
            from closeBracket in x5DParser.Instance
            select new Option(openBracket, prefixCwsps, alternation, suffixCwsps, closeBracket);
    }
}
