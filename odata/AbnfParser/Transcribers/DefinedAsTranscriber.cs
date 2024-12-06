namespace AbnfParser.Transcribers
{
    using System.Text;

    using AbnfParser.CstNodes;
    using AbnfParser.Transcribers.Core;
    using Root;

    public sealed class DefinedAsTranscriber : DefinedAs.Visitor<Void, StringBuilder>
    {
        private DefinedAsTranscriber()
        {
        }

        public static DefinedAsTranscriber Instance { get; } = new DefinedAsTranscriber();

        protected internal override Void Accept(DefinedAs.Declaration node, StringBuilder context)
        {
            foreach (var prefixCwsp in node.PrefixCwsps)
            {
                CwspTranscriber.Instance.Visit(prefixCwsp, context);
            }

            x3DTranscriber.Instance.Transcribe(node.Equals, context);
            foreach (var suffixCwsp in node.SuffixCwsps)
            {
                CwspTranscriber.Instance.Visit(suffixCwsp, context);
            }

            return default;
        }

        protected internal override Void Accept(DefinedAs.Incremental node, StringBuilder context)
        {
            foreach (var prefixCwsp in node.PrefixCwsps)
            {
                CwspTranscriber.Instance.Visit(prefixCwsp, context);
            }

            x3DTranscriber.Instance.Transcribe(node.Equals, context);
            x2FTranscriber.Instance.Transcribe(node.Slash, context);
            foreach (var suffixCwsp in node.SuffixCwsps)
            {
                CwspTranscriber.Instance.Visit(suffixCwsp, context);
            }

            return default;
        }
    }
}
