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
                new Abnf.Inners._ʺx3Bʺ(
                    x3BConverter.Instance.Convert(comment.Semicolon)),
                comment.Inners.Select(inner =>
                    InnerConverter.Instance.Visit(inner, default)),
                CrLfConverter.Instance.Convert(comment.Crlf));
        }

        private sealed class InnerConverter : AbnfParser.CstNodes.Comment.Inner.Visitor<GeneratorV3.Abnf.Inners._ⲤWSPⳆVCHARↃ, Root.Void>
        {
            private InnerConverter()
            {
            }

            public static InnerConverter Instance { get; } = new InnerConverter();

            protected internal override Abnf.Inners._ⲤWSPⳆVCHARↃ Accept(Comment.Inner.InnerWsp node, Void context)
            {
                return new Abnf.Inners._ⲤWSPⳆVCHARↃ(
                    new Abnf.Inners._WSPⳆVCHAR._WSP(
                        WspConverter.Instance.Visit(node.Wsp, context)));
            }

            protected internal override Abnf.Inners._ⲤWSPⳆVCHARↃ Accept(Comment.Inner.InnerVchar node, Void context)
            {
                return new Abnf.Inners._ⲤWSPⳆVCHARↃ(
                    new Abnf.Inners._WSPⳆVCHAR._VCHAR(
                        VcharConverter.Instance.Visit(node.Vchar, context)));
            }
        }
    }
}
