namespace AbnfParser.CombinatorParsers.Core
{
    using AbnfParser.CstNodes.Core;
    using Sprache;

    public static class CrlfParser
    {
        public static Parser<Crlf> Instance { get; } =
            from cr in CrParser.Instance
            from lf in LfParser.Instance
            select new Crlf(cr, lf);
    }
}
