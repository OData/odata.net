namespace AbnfParser.Transcribers.Core
{
    using System.Text;

    using AbnfParser.CstNodes.Core;
    using Root;

    public sealed class LfTranscriber
    {
        private LfTranscriber()
        {
        }

        public static LfTranscriber Instance { get; } = new LfTranscriber();

        public Void Transcribe(Lf node, StringBuilder context)
        {
            x0ATranscriber.Instance.Transcribe(node.X0A, context);
            return default;
        }
    }
}
