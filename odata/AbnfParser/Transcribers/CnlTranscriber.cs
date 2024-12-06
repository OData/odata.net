namespace AbnfParser.Transcribers
{
    using System.Text;

    using AbnfParser.CstNodes;
    using AbnfParser.Transcribers.Core;
    using Root;

    public sealed class CnlTranscriber : Cnl.Visitor<Void, StringBuilder>
    {
        private CnlTranscriber()
        {
        }

        public static CnlTranscriber Instance { get; } = new CnlTranscriber();

        protected internal override Void Accept(Cnl.Comment node, StringBuilder context)
        {
            CommentTranscriber.Instance.Transcribe(node.Value, context);
            return default;
        }

        protected internal override Void Accept(Cnl.Newline node, StringBuilder context)
        {
            CrlfTranscriber.Instance.Transcribe(node.Crlf, context);
            return default;
        }
    }
}
