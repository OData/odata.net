namespace AbnfParser.CombinatorParsers
{
    using AbnfParser.CombinatorParsers.Core;
    using AbnfParser.CstNodes;
    using AbnfParser.CstNodes.Core;
    using Sprache;

    public static class CommentParser
    {
        public static Parser<Comment> Instance { get; }

        public static class InnerParser
        {
            public static Parser<Comment.Inner.InnerWsp> InnerWsp { get; } =
                from wsp in WspParser.Instance
                select new Comment.Inner.InnerWsp(wsp);

            public static Parser<Comment.Inner.InnerVchar> InnerVchar { get; } =
                from vchar in VcharParser.Instance
                select new Comment.Inner.InnerVchar(vchar);

            public static Parser<Comment.Inner> Instance { get; } =
                InnerWsp
                .Or<Comment.Inner>(InnerVchar);
        }
    }
}
