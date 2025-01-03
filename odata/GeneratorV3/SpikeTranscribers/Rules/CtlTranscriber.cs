namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;
    using GeneratorV3.SpikeTranscribers.Inners;
    using Root;

    public sealed class CtlTranscriber : ITranscriber<_CTL>
    {
        private CtlTranscriber()
        {
        }

        public static CtlTranscriber Instance { get; } = new CtlTranscriber();

        public void Transcribe(_CTL value, StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }

        private sealed class Visitor : _CTL.Visitor<Root.Void, StringBuilder>
        {
            private Visitor()
            {
            }

            public static Visitor Instance { get; } = new Visitor();

            protected internal override Void Accept(_CTL._Ⰳx00ⲻ1F node, StringBuilder context)
            {
                _Ⰳx00ⲻ1FTranscriber.Instance.Transcribe(node._Ⰳx00ⲻ1F_1, context);

                return default;
            }

            protected internal override Void Accept(_CTL._Ⰳx7F node, StringBuilder context)
            {
                context.Append((char)0x7F);

                return default;
            }
        }
    }
}
