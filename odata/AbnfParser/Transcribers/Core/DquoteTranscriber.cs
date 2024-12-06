namespace AbnfParser.Transcribers.Core
{
    using System.Text;

    using AbnfParser.CstNodes.Core;
    using Root;

    public sealed class DquoteTranscriber
    {
        private DquoteTranscriber()
        {
        }

        public static DquoteTranscriber Instance { get; } = new DquoteTranscriber();

        public Void Transcribe(Dquote node, StringBuilder context)
        {
            x22Transcriber.Instance.Transcribe(node.Value, context);
            return default;
        }
    }
}
