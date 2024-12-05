namespace AbnfParser.Transcribers.Core
{
    using System.Text;

    using AbnfParser.CstNodes.Core;
    using Root;

    public sealed class CrTranscriber
    {
        private CrTranscriber()
        {
        }

        public static CrTranscriber Instance { get; } = new CrTranscriber();

        public Void Transcribe(Cr node, StringBuilder context)
        {
            x0DTranscriber.Instance.Transcribe(node.X0D, context);
            return default;
        }
    }
}
