namespace AbnfParser.CombinatorParsers
{
    using AbnfParser.CombinatorParsers.Core;
    using AbnfParser.CstNodes;
    using Sprache;

    public static class ElementParser
    {
        public static Parser<Element.RuleName> RuleName { get; } =
            from value in RuleNameParser.Instance
            select new Element.RuleName(value);

        public static Parser<Element.Group> Group { get; } =
            from value in GroupParser.Instance
            select new Element.Group(value);

        public static Parser<Element.Option> Option { get; } =
            from value in OptionParser.Instance
            select new Element.Option(value);

        public static Parser<Element.CharVal> CharVal { get; } =
            from value in CharValParser.Instance
            select new Element.CharVal(value);
    }
}
