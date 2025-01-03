namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;
    using Root;

    public sealed class _Ⰳx61ⲻ7ATranscriber : ITranscriber<Inners._Ⰳx61ⲻ7A>
    {
        private _Ⰳx61ⲻ7ATranscriber()
        {
        }

        public static _Ⰳx61ⲻ7ATranscriber Instance { get; } = new _Ⰳx61ⲻ7ATranscriber();

        public void Transcribe(Inners._Ⰳx61ⲻ7A value, StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }

        private sealed class Visitor : Inners._Ⰳx61ⲻ7A.Visitor<Root.Void, StringBuilder>
        {
            private Visitor()
            {
            }

            public static Visitor Instance { get; } = new Visitor();

            protected internal override Void Accept(Inners._Ⰳx61ⲻ7A._61 node, StringBuilder context)
            {
                context.Append((char)0x61);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx61ⲻ7A._62 node, StringBuilder context)
            {
                context.Append((char)0x62);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx61ⲻ7A._63 node, StringBuilder context)
            {
                context.Append((char)0x63);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx61ⲻ7A._64 node, StringBuilder context)
            {
                context.Append((char)0x64);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx61ⲻ7A._65 node, StringBuilder context)
            {
                context.Append((char)0x65);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx61ⲻ7A._66 node, StringBuilder context)
            {
                context.Append((char)0x66);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx61ⲻ7A._67 node, StringBuilder context)
            {
                context.Append((char)0x67);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx61ⲻ7A._68 node, StringBuilder context)
            {
                context.Append((char)0x68);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx61ⲻ7A._69 node, StringBuilder context)
            {
                context.Append((char)0x69);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx61ⲻ7A._6A node, StringBuilder context)
            {
                context.Append((char)0x6A);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx61ⲻ7A._6B node, StringBuilder context)
            {
                context.Append((char)0x6B);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx61ⲻ7A._6C node, StringBuilder context)
            {
                context.Append((char)0x6C);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx61ⲻ7A._6D node, StringBuilder context)
            {
                context.Append((char)0x6D);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx61ⲻ7A._6E node, StringBuilder context)
            {
                context.Append((char)0x6E);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx61ⲻ7A._6F node, StringBuilder context)
            {
                context.Append((char)0x6F);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx61ⲻ7A._70 node, StringBuilder context)
            {
                context.Append((char)0x70);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx61ⲻ7A._71 node, StringBuilder context)
            {
                context.Append((char)0x71);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx61ⲻ7A._72 node, StringBuilder context)
            {
                context.Append((char)0x72);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx61ⲻ7A._73 node, StringBuilder context)
            {
                context.Append((char)0x73);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx61ⲻ7A._74 node, StringBuilder context)
            {
                context.Append((char)0x74);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx61ⲻ7A._75 node, StringBuilder context)
            {
                context.Append((char)0x75);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx61ⲻ7A._76 node, StringBuilder context)
            {
                context.Append((char)0x76);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx61ⲻ7A._77 node, StringBuilder context)
            {
                context.Append((char)0x77);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx61ⲻ7A._78 node, StringBuilder context)
            {
                context.Append((char)0x78);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx61ⲻ7A._79 node, StringBuilder context)
            {
                context.Append((char)0x79);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx61ⲻ7A._7A node, StringBuilder context)
            {
                context.Append((char)0x7A);

                return default;
            }
        }
    }
}
