namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;
    using GeneratorV3.SpikeTranscribers.Inners;
    using Root;

    public sealed class _cⲻwspTranscriber : ITranscriber<_cⲻwsp>
    {
        private _cⲻwspTranscriber()
        {
        }

        public static _cⲻwspTranscriber Instance { get; } = new _cⲻwspTranscriber();

        public void Transcribe(_cⲻwsp value, StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }

        private sealed class Visitor : _cⲻwsp.Visitor<Root.Void, StringBuilder>
        {
            private Visitor()
            {
            }

            public static Visitor Instance { get; } = new Visitor();

            protected internal override Void Accept(_cⲻwsp._WSP node, StringBuilder context)
            {
                WspTranscriber.Instance.Transcribe(node._WSP_1, context);

                return default;
            }

            protected internal override Void Accept(_cⲻwsp._Ⲥcⲻnl_WSPↃ node, StringBuilder context)
            {
                _Ⲥcⲻnl_WSPↃTranscriber.Instance.Transcribe(node._Ⲥcⲻnl_WSPↃ_1, context);

                return default;
            }
        }
    }
}
