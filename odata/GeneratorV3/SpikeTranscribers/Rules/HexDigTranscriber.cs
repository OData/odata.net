namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;
    using Root;

    public sealed class HexDigTranscriber : ITranscriber<_HEXDIG>
    {
        private HexDigTranscriber()
        {
        }

        public static HexDigTranscriber Instance { get; } = new HexDigTranscriber();

        public void Transcribe(_HEXDIG value, StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }

        private sealed class Visitor : _HEXDIG.Visitor<Root.Void, StringBuilder>
        {
            private Visitor()
            {
            }

            public static Visitor Instance { get; } = new Visitor();

            protected internal override Void Accept(_HEXDIG._DIGIT node, StringBuilder context)
            {
                DigitTranscriber.Instance.Transcribe(node._DIGIT_1, context);

                return default;
            }

            protected internal override Void Accept(_HEXDIG._ʺx41ʺ node, StringBuilder context)
            {
                context.Append((char)0x41);

                return default;
            }

            protected internal override Void Accept(_HEXDIG._ʺx42ʺ node, StringBuilder context)
            {
                context.Append((char)0x42);

                return default;
            }

            protected internal override Void Accept(_HEXDIG._ʺx43ʺ node, StringBuilder context)
            {
                context.Append((char)0x43);

                return default;
            }

            protected internal override Void Accept(_HEXDIG._ʺx44ʺ node, StringBuilder context)
            {
                context.Append((char)0x44);

                return default;
            }

            protected internal override Void Accept(_HEXDIG._ʺx45ʺ node, StringBuilder context)
            {
                context.Append((char)0x45);

                return default;
            }

            protected internal override Void Accept(_HEXDIG._ʺx46ʺ node, StringBuilder context)
            {
                context.Append((char)0x46);

                return default;
            }
        }
    }
}
