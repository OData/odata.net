namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;
    using GeneratorV3.SpikeTranscribers.Inners;

    public sealed class AlphaTranscriber : ITranscriber<_ALPHA>
    {
        private AlphaTranscriber()
        {
        }

        public static AlphaTranscriber Instance { get; } = new AlphaTranscriber();

        public void Transcribe(_ALPHA value, StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }

        private sealed class Visitor : _ALPHA.Visitor<Root.Void, StringBuilder>
        {
            private Visitor()
            {
            }

            public static Visitor Instance { get; } = new Visitor();

            protected internal override Root.Void Accept(_ALPHA._Ⰳx41ⲻ5A node, StringBuilder context)
            {
                _Ⰳx41ⲻ5ATranscriber.Instance.Transcribe(node._Ⰳx41ⲻ5A_1, context);
                return default;
            }

            protected internal override Root.Void Accept(_ALPHA._Ⰳx61ⲻ7A node, StringBuilder context)
            {
                _Ⰳx61ⲻ7ATranscriber.Instance.Transcribe(node._Ⰳx61ⲻ7A_1, context);
                return default;
            }
        }
    }
}
