namespace AbnfParser.Transcribers
{
    using System.Text;

    using AbnfParser.CstNodes;
    using Root;

    public sealed class ConcatenationTranscriber
    {
        private ConcatenationTranscriber()
        {
        }

        public static ConcatenationTranscriber Instance { get; } = new ConcatenationTranscriber();

        public Void Transcribe(Concatenation node, StringBuilder context)
        {
            RepetitionTranscriber.Instance.Visit(node.Repetition, context);
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

            public Void Transcribe(Concatenation.Inner node, StringBuilder context)
            {
                foreach (var cwsp in node.Cwsps)
                {
                    CwspTranscriber.Instance.Visit(cwsp, context);
                }

                RepetitionTranscriber.Instance.Visit(node.Repetition, context);
                return default;
            }
        }
    }
}
