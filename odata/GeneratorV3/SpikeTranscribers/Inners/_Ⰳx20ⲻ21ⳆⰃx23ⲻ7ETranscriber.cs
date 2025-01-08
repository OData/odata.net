namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;
    using Root;

    public sealed class _Ⰳx20ⲻ21ⳆⰃx23ⲻ7ETranscriber : ITranscriber<Inners._Ⰳx20ⲻ21ⳆⰃx23ⲻ7E>
    {
        private _Ⰳx20ⲻ21ⳆⰃx23ⲻ7ETranscriber()
        {
        }

        public static _Ⰳx20ⲻ21ⳆⰃx23ⲻ7ETranscriber Instance { get; } = new _Ⰳx20ⲻ21ⳆⰃx23ⲻ7ETranscriber();

        public void Transcribe(Inners._Ⰳx20ⲻ21ⳆⰃx23ⲻ7E value, StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }

        private sealed class Visitor : Inners._Ⰳx20ⲻ21ⳆⰃx23ⲻ7E.Visitor<Root.Void, StringBuilder>
        {
            private Visitor()
            {
            }

            public static Visitor Instance { get; } = new Visitor();

            protected internal override Void Accept(Inners._Ⰳx20ⲻ21ⳆⰃx23ⲻ7E._Ⰳx20ⲻ21 node, StringBuilder context)
            {
                _Ⰳx20ⲻ21Transcriber.Instance.Transcribe(node._Ⰳx20ⲻ21_1, context);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx20ⲻ21ⳆⰃx23ⲻ7E._Ⰳx23ⲻ7E node, StringBuilder context)
            {
                _Ⰳx23ⲻ7ETranscriber.Instance.Transcribe(node._Ⰳx23ⲻ7E_1, context);

                return default;
            }
        }
    }
}
