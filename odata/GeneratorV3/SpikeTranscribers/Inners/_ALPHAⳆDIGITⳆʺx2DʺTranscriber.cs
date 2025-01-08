namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;
    using GeneratorV3.SpikeTranscribers.Rules;
    using Root;

    public sealed class _ALPHAⳆDIGITⳆʺx2DʺTranscriber : ITranscriber<Inners._ALPHAⳆDIGITⳆʺx2Dʺ>
    {
        private _ALPHAⳆDIGITⳆʺx2DʺTranscriber()
        {
        }

        public static _ALPHAⳆDIGITⳆʺx2DʺTranscriber Instance { get; } = new _ALPHAⳆDIGITⳆʺx2DʺTranscriber();

        public void Transcribe(Inners._ALPHAⳆDIGITⳆʺx2Dʺ value, StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }

        private sealed class Visitor : Inners._ALPHAⳆDIGITⳆʺx2Dʺ.Visitor<Root.Void, StringBuilder>
        {
            private Visitor()
            {
            }

            public static Visitor Instance { get; } = new Visitor();

            protected internal override Void Accept(Inners._ALPHAⳆDIGITⳆʺx2Dʺ._ALPHA node, StringBuilder context)
            {
                AlphaTranscriber.Instance.Transcribe(node._ALPHA_1, context);

                return default;
            }

            protected internal override Void Accept(Inners._ALPHAⳆDIGITⳆʺx2Dʺ._DIGIT node, StringBuilder context)
            {
                DigitTranscriber.Instance.Transcribe(node._DIGIT_1, context);

                return default;
            }

            protected internal override Void Accept(Inners._ALPHAⳆDIGITⳆʺx2Dʺ._ʺx2Dʺ node, StringBuilder context)
            {
                _ʺx2DʺTranscriber.Instance.Transcribe(node._ʺx2Dʺ_1, context);

                return default;
            }
        }
    }
}
