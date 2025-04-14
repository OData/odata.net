namespace AbnfParser.CombinatorParsers
{
    using AbnfParser.CombinatorParsers.Core;
    using AbnfParser.CstNodes;
    using Sprache;

    public static class CnlParser
    {
        public static Parser<Cnl.Comment> Comment { get; } =
            from comment in CommentParser.Instance
            select new Cnl.Comment(comment);

        public static Parser<Cnl.Newline> NewLine { get; } =
            from crlf in CrlfParser.Instance
            select new Cnl.Newline(crlf);

        public static Parser<Cnl> Instance { get; } =
            Comment
            .Or<Cnl>(NewLine);
    }
}
