namespace AbnfParser.Transcribers.Core
{
    using System.Text;

    using AbnfParser.CstNodes.Core;
    using Root;

    public sealed class HtabTranscriber
    {
        private HtabTranscriber()
        {
        }

        public static HtabTranscriber Instance { get; } = new HtabTranscriber();

        public Void Transcribe(Htab node, StringBuilder context)
        {
            x09Transcriber.Instance.Transcribe(node.Value, context);
            return default;
        }
    }
}
