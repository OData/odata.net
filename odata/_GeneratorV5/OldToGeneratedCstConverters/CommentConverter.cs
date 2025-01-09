namespace _GeneratorV5.OldToGeneratedCstConverters
{
    using System.Linq;

    public sealed class CommentConverter
    {
        private CommentConverter()
        {
        }

        public static CommentConverter Instance { get; } = new CommentConverter();

        public __Generated.CstNodes.Rules._comment Convert(AbnfParser.CstNodes.Comment comment)
        {
            return new __Generated.CstNodes.Rules._comment(
                new __Generated.CstNodes.Inners._ʺx3Bʺ(
                    x3BConverter.Instance.Convert(comment.Semicolon)),
                comment.Inners.Select(inner =>
                    InnerConverter.Instance.Visit(inner, default)),
                CrLfConverter.Instance.Convert(comment.Crlf));
        }

        private sealed class InnerConverter : AbnfParser.CstNodes.Comment.Inner.Visitor<__Generated.CstNodes.Inners._ⲤWSPⳆVCHARↃ, Root.Void>
        {
            private InnerConverter()
            {
            }

            public static InnerConverter Instance { get; } = new InnerConverter();

            protected internal override __Generated.CstNodes.Inners._ⲤWSPⳆVCHARↃ Accept(AbnfParser.CstNodes.Comment.Inner.InnerWsp node, Root.Void context)
            {
                return new __Generated.CstNodes.Inners._ⲤWSPⳆVCHARↃ(
                    new __Generated.CstNodes.Inners._WSPⳆVCHAR._WSP(
                        WspConverter.Instance.Visit(node.Wsp, context)));
            }

            protected internal override __Generated.CstNodes.Inners._ⲤWSPⳆVCHARↃ Accept(AbnfParser.CstNodes.Comment.Inner.InnerVchar node, Root.Void context)
            {
                return new __Generated.CstNodes.Inners._ⲤWSPⳆVCHARↃ(
                    new __Generated.CstNodes.Inners._WSPⳆVCHAR._VCHAR(
                        VcharConverter.Instance.Visit(node.Vchar, context)));
            }
        }
    }
}
