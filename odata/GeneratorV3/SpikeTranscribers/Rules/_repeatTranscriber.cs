namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;
    using GeneratorV3.SpikeTranscribers.Inners;
    using Root;

    public sealed class _repeatTranscriber : ITranscriber<_repeat>
    {
        private _repeatTranscriber()
        {
        }

        public static _repeatTranscriber Instance { get; } = new _repeatTranscriber();

        public void Transcribe(_repeat value, StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }

        private sealed class Visitor : _repeat.Visitor<Root.Void, StringBuilder>
        {
            private Visitor()
            {
            }

            public static Visitor Instance { get; } = new Visitor();

            protected internal override Void Accept(_repeat._1ЖDIGIT node, StringBuilder context)
            {
                foreach (var _DIGIT in node._DIGIT_1)
                {
                    DigitTranscriber.Instance.Transcribe(_DIGIT, context);
                }

                return default;
            }

            protected internal override Void Accept(_repeat._ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ node, StringBuilder context)
            {
                _ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃTranscriber.Instance.Transcribe(node._ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ_1, context);

                return default;
            }
        }
    }
}
