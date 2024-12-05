namespace AbnfParser.Transcribers
{
    using System.Text;

    using AbnfParser.CstNodes;
    using AbnfParser.Transcribers.Core;
    using Root;

    public sealed class OptionTranscriber
    {
        private OptionTranscriber()
        {
        }

        public static OptionTranscriber Instance { get; } = new OptionTranscriber();

        public Void Transcribe(Option node, StringBuilder context)
        {
            x5BTranscriber.Instance.Transcribe(node.OpenBracket, context);
            foreach (var prefixCwsp in node.PrefixCwsps)
            {
                CwspTranscriber.Instance.Visit(prefixCwsp, context);
            }

            AlternationTranscriber.Instance.Transcribe(node.Alternation, context);
            foreach (var suffixCwsp in node.SuffixCwsps)
            {
                CwspTranscriber.Instance.Visit(suffixCwsp, context);
            }

            x5DTranscriber.Instance.Transcribe(node.CloseBracket, context);
            return default;
        }
    }
}
