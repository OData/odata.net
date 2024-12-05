namespace AbnfParser.Transcribers
{
    using System.Text;

    using AbnfParser.CstNodes;
    using AbnfParser.Transcribers.Core;
    using Root;

    public sealed class GroupTranscriber
    {
        private GroupTranscriber()
        {
        }

        public static GroupTranscriber Instance { get; } = new GroupTranscriber();

        public Void Transcribe(Group node, StringBuilder context)
        {
            x28Transcriber.Instance.Transcribe(node.OpenParenthesis, context);
            foreach (var prefixCwsp in node.PrefixCwsps)
            {
                CwspTranscriber.Instance.Visit(prefixCwsp, context);
            }

            AlternationTranscriber.Instance.Transcribe(node.Alternation, context);

            foreach (var suffixCwsp in node.SuffixCwsps)
            {
                CwspTranscriber.Instance.Visit(suffixCwsp, context);
            }

            x29Transcriber.Instance.Transcribe(node.CloseParenthesis, context);

            return default;
        }
    }
}
