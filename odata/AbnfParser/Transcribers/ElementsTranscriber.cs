namespace AbnfParser.Transcribers
{
    using System.Text;

    using AbnfParser.CstNodes;
    using Root;

    public sealed class ElementsTranscriber
    {
        private ElementsTranscriber()
        {
        }

        public static ElementsTranscriber Instance { get; } = new ElementsTranscriber();

        public Void Transcribe(Elements node, StringBuilder context)
        {
            AlternationTranscriber.Instance.Transcribe(node.Alternation, context);
            foreach (var cwsp in node.Cwsps)
            {
                CwspTranscriber.Instance.Visit(cwsp, context);
            }

            return default;
        }
    }
}
