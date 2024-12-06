namespace AbnfParser.Transcribers.Core
{
    using System.Text;

    using AbnfParser.CstNodes.Core;
    using Root;

    public sealed class SpTranscriber
    {
        private SpTranscriber()
        {
        }

        public static SpTranscriber Instance { get; } = new SpTranscriber();

        public Void Transcribe(Sp node, StringBuilder context)
        {
            x20Transcriber.Instance.Transcribe(node.Value, context);
            return default;
        }
    }
}
