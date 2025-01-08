namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;
    using Root;

    public sealed class _Ⰳx20ⲻ21Transcriber : ITranscriber<Inners._Ⰳx20ⲻ21>
    {
        private _Ⰳx20ⲻ21Transcriber()
        {
        }

        public static _Ⰳx20ⲻ21Transcriber Instance { get; } = new _Ⰳx20ⲻ21Transcriber();

        public void Transcribe(Inners._Ⰳx20ⲻ21 value, StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }

        private sealed class Visitor : Inners._Ⰳx20ⲻ21.Visitor<Root.Void, StringBuilder>
        {
            private Visitor()
            {
            }

            public static Visitor Instance { get; } = new Visitor();

            protected internal override Void Accept(Inners._Ⰳx20ⲻ21._20 node, StringBuilder context)
            {
                context.Append((char)0x20);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx20ⲻ21._21 node, StringBuilder context)
            {
                context.Append((char)0x21);

                return default;
            }
        }
    }
}
