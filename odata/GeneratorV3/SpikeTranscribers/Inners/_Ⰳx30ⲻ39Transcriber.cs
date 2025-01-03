namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;
    using Root;

    public sealed class _Ⰳx30ⲻ39Transcriber : ITranscriber<Inners._Ⰳx30ⲻ39>
    {
        private _Ⰳx30ⲻ39Transcriber()
        {
        }

        public static _Ⰳx30ⲻ39Transcriber Instance { get; } = new _Ⰳx30ⲻ39Transcriber();

        public void Transcribe(Inners._Ⰳx30ⲻ39 value, StringBuilder builder)
        {
            return Visitor.Instance.Visit(value, builder);
        }

        private sealed class Visitor : Inners._Ⰳx30ⲻ39.Visitor<Root.Void, StringBuilder>
        {
            private Visitor()
            {
            }

            public static Visitor Instance { get; } = new Visitor();

            protected internal override Void Accept(Inners._Ⰳx30ⲻ39._30 node, StringBuilder context)
            {
                context.Append((char)0x30);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx30ⲻ39._31 node, StringBuilder context)
            {
                context.Append((char)0x31);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx30ⲻ39._32 node, StringBuilder context)
            {
                context.Append((char)0x32);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx30ⲻ39._33 node, StringBuilder context)
            {
                context.Append((char)0x33);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx30ⲻ39._34 node, StringBuilder context)
            {
                context.Append((char)0x34);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx30ⲻ39._35 node, StringBuilder context)
            {
                context.Append((char)0x35);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx30ⲻ39._36 node, StringBuilder context)
            {
                context.Append((char)0x36);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx30ⲻ39._37 node, StringBuilder context)
            {
                context.Append((char)0x37);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx30ⲻ39._38 node, StringBuilder context)
            {
                context.Append((char)0x38);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx30ⲻ39._39 node, StringBuilder context)
            {
                context.Append((char)0x39);

                return default;
            }
        }
    }
}
