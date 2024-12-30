namespace GeneratorV3.OldToNewConverters
{
    using AbnfParser.CstNodes;
    using GeneratorV3.OldToNewConverters.Core;
    using Root;
    using System.Linq;

    public sealed class CommentConverter
    {
        private CommentConverter()
        {
        }

        public static CommentConverter Instance { get; } = new CommentConverter();

        public GeneratorV3.Abnf._comment Convert(AbnfParser.CstNodes.Comment comment)
        {
            return new Abnf._comment(
                new Abnf.Inners._doublequotex3Bdoublequote(
                    x3BConverter.Instance.Convert(comment.Semicolon)),
                comment.Inners.Select(inner =>
                    InnerConverter.Instance.Visit(inner, default)),
                CrLfConverter.Instance.Convert(comment.Crlf));
        }

        private sealed class InnerConverter : AbnfParser.CstNodes.Comment.Inner.Visitor<GeneratorV3.Abnf.Inners._openWSPⳆVCHARↃ, Root.Void>
        {
            private InnerConverter()
            {
            }

            public static InnerConverter Instance { get; } = new InnerConverter();

            protected internal override Abnf.Inners._openWSPⳆVCHARↃ Accept(Comment.Inner.InnerWsp node, Void context)
            {
                return new Abnf.Inners._openWSPⳆVCHARↃ(
                    new Abnf.Inners._WSPⳆVCHAR._WSP(
                        WspConverter.Instance.Visit(node.Wsp, context)));
            }

            protected internal override Abnf.Inners._openWSPⳆVCHARↃ Accept(Comment.Inner.InnerVchar node, Void context)
            {
                return new Abnf.Inners._openWSPⳆVCHARↃ(
                    new Abnf.Inners._WSPⳆVCHAR._VCHAR(
                        VcharConverter.Instance.Visit(node.Vchar, context)));
            }
        }
    }
}
