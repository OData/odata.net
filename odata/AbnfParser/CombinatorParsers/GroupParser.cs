namespace AbnfParser.CombinatorParsers
{
    using AbnfParser.CombinatorParsers.Core;
    using AbnfParser.CstNodes;
    using Sprache;

    public static class GroupParser
    {
        public static Parser<Group> Instance { get; } =
            from openParenthesis in x28Parser.Instance
            from prefixCwsps in CwspParser.Instance.Many()
            from alternation in AlternationParser.Instance
            from suffixCwsps in CwspParser.Instance.Many()
            from closeParenthesis in x29Parser.Instance
            select new Group(openParenthesis, prefixCwsps, alternation, suffixCwsps, closeParenthesis);
    }
}
