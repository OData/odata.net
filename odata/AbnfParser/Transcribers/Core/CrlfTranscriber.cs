namespace AbnfParser.Transcribers.Core
{
    using System.Text;

    using AbnfParser.CstNodes.Core;
    using Root;

    public sealed class CrlfTranscriber
    {
        private CrlfTranscriber()
        {
        }

        public static CrlfTranscriber Instance { get; } = new CrlfTranscriber();

        public Void Transcribe(Crlf node, StringBuilder context)
        {
            CrTranscriber.Instance.Transcribe(node.Cr, context);
            LfTranscriber.Instance.Transcribe(node.Lf, context);
            return default;
        }
    }
}
