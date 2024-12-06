namespace AbnfParser.Transcribers
{
    using System.Text;

    using AbnfParser.CstNodes;
    using AbnfParser.Transcribers.Core;
    using Root;

    public sealed class AlternationTranscriber
    {
        private AlternationTranscriber()
        {
        }

        public static AlternationTranscriber Instance { get; } = new AlternationTranscriber();

        public Void Transcribe(Alternation node, StringBuilder context)
        {
            ConcatenationTranscriber.Instance.Transcribe(node.Concatenation, context);
            foreach (var inner in node.Inners)
            {
                InnerTranscriber.Instance.Transcribe(inner, context);
            }

            return default;
        }

        public sealed class InnerTranscriber
        {
            private InnerTranscriber()
            {
            }

            public static InnerTranscriber Instance { get; } = new InnerTranscriber();

            public Void Transcribe(Alternation.Inner node, StringBuilder context)
            {
                foreach (var prefixCwsp in node.PrefixCwsps)
                {
                    CwspTranscriber.Instance.Visit(prefixCwsp, context);
                }

                x2FTranscriber.Instance.Transcribe(node.Slash, context);

                foreach (var suffixCwsp in node.SuffixCwsps)
                {
                    CwspTranscriber.Instance.Visit(suffixCwsp, context);
                }

                ConcatenationTranscriber.Instance.Transcribe(node.Concatenation, context);
                return default;
            }
        }
    }
}
