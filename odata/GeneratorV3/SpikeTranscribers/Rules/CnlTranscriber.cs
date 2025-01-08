namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;
    using GeneratorV3.Abnf;
    using Root;

    public sealed class CnlTranscriber : ITranscriber<_cⲻnl>
    {
        private CnlTranscriber()
        {
        }

        public static CnlTranscriber Instance { get; } = new CnlTranscriber();

        public void Transcribe(_cⲻnl value, StringBuilder builder)
        {
        }

        private sealed class Visitor : _cⲻnl.Visitor<Root.Void, StringBuilder>
        {
            private Visitor()
            {
            }

            public static Visitor Instance { get; } = new Visitor();

            protected internal override Void Accept(_cⲻnl._comment node, StringBuilder context)
            {
                CommentTranscriber.Instance.Transcribe(node._comment_1, context);

                return default;
            }

            protected internal override Void Accept(_cⲻnl._CRLF node, StringBuilder context)
            {
                CrLfTranscriber.Instance.Transcribe(node._CRLF_1, context);

                return default;
            }
        }
    }
}
