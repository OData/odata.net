namespace AbnfParser.Transcribers
{
    using System.Text;

    using AbnfParser.CstNodes;
    using AbnfParser.Transcribers.Core;
    using Root;

    public sealed class CommentTranscriber
    {
        private CommentTranscriber()
        {
        }

        public static CommentTranscriber Instance { get; } = new CommentTranscriber();

        public Void Transcribe(Comment node, StringBuilder context)
        {
            x3BTranscriber.Instance.Transcribe(node.Semicolon, context);
            foreach (var inner in node.Inners)
            {
                InnerTranscriber.Instance.Visit(inner, context);
            }

            CrlfTranscriber.Instance.Transcribe(node.Crlf, context);
            return default;
        }

        public sealed class InnerTranscriber : Comment.Inner.Visitor<Void, StringBuilder>
        {
            private InnerTranscriber()
            {
            }

            public static InnerTranscriber Instance { get; } = new InnerTranscriber();

            protected internal override Void Accept(Comment.Inner.InnerWsp node, StringBuilder context)
            {
                WspTranscriber.Instance.Visit(node.Wsp, context);
                return default;
            }

            protected internal override Void Accept(Comment.Inner.InnerVchar node, StringBuilder context)
            {
                VcharTranscriber.Instance.Visit(node.Vchar, context);
                return default;
            }
        }
    }
}
