namespace AbnfParser.CombinatorParsers
{
    using AbnfParser.CombinatorParsers.Core;
    using AbnfParser.CstNodes;
    using Sprache;

    public static class GroupParser
    {
        public static Parser<Group> Instance { get; } =
            from openParenthesis in x28Parser.Instance
            from prefixCwsps in Parse.Many(CwspParser.Instance)
            from alternation in AlternationParser.Instance
            from suffixCwsps in Parse.Many(CwspParser.Instance)
            from closeParenthesis in x29Parser.Instance
            select new Group(openParenthesis, prefixCwsps, alternation, suffixCwsps, closeParenthesis);
    }
}
