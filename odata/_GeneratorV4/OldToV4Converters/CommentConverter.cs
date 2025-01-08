namespace _GeneratorV4.OldToV4Converters
{
    using _GeneratorV4.Abnf.CstNodes;
    using AbnfParser.CstNodes;
    using Root;
    using System.Linq;

    public sealed class CommentConverter
    {
        private CommentConverter()
        {
        }

        public static CommentConverter Instance { get; } = new CommentConverter();

        public _comment Convert(AbnfParser.CstNodes.Comment comment)
        {
            return new _comment(
                new Inners._ʺx3Bʺ(
                    x3BConverter.Instance.Convert(comment.Semicolon)),
                comment.Inners.Select(inner =>
                    InnerConverter.Instance.Visit(inner, default)),
                CrLfConverter.Instance.Convert(comment.Crlf));
        }

        private sealed class InnerConverter : AbnfParser.CstNodes.Comment.Inner.Visitor<Inners._ⲤWSPⳆVCHARↃ, Root.Void>
        {
            private InnerConverter()
            {
            }

            public static InnerConverter Instance { get; } = new InnerConverter();

            protected internal override Inners._ⲤWSPⳆVCHARↃ Accept(Comment.Inner.InnerWsp node, Void context)
            {
                return new Inners._ⲤWSPⳆVCHARↃ(
                    new Inners._WSPⳆVCHAR._WSP(
                        WspConverter.Instance.Visit(node.Wsp, context)));
            }

            protected internal override Inners._ⲤWSPⳆVCHARↃ Accept(Comment.Inner.InnerVchar node, Void context)
            {
                return new Inners._ⲤWSPⳆVCHARↃ(
                    new Inners._WSPⳆVCHAR._VCHAR(
                        VcharConverter.Instance.Visit(node.Vchar, context)));
            }
        }
    }
}
