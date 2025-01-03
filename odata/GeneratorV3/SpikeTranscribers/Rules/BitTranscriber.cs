namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;
    using Root;

    public sealed class BitTranscriber : ITranscriber<_BIT>
    {
        private BitTranscriber()
        {
        }

        public static BitTranscriber Instance { get; } = new BitTranscriber();

        public void Transcribe(_BIT value, StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }

        private sealed class Visitor : _BIT.Visitor<Root.Void, StringBuilder>
        {
            private Visitor()
            {
            }

            public static Visitor Instance { get; } = new Visitor();

            protected internal override Void Accept(_BIT._ʺx30ʺ node, StringBuilder context)
            {
                context.Append((char)0x30);

                return default;
            }

            protected internal override Void Accept(_BIT._ʺx31ʺ node, StringBuilder context)
            {
                context.Append((char)0x31);

                return default;
            }
        }
    }
}
