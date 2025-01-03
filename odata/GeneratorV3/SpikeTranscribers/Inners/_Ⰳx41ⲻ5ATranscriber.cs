namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;
    using Root;

    public sealed class _Ⰳx41ⲻ5ATranscriber : ITranscriber<Inners._Ⰳx41ⲻ5A>
    {
        private _Ⰳx41ⲻ5ATranscriber()
        {
        }

        public static _Ⰳx41ⲻ5ATranscriber Instance { get; } = new _Ⰳx41ⲻ5ATranscriber();

        public void Transcribe(Inners._Ⰳx41ⲻ5A value, StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }

        private sealed class Visitor : Inners._Ⰳx41ⲻ5A.Visitor<Root.Void, StringBuilder>
        {
            private Visitor()
            {
            }

            public static Visitor Instance { get; } = new Visitor();

            protected internal override Void Accept(Inners._Ⰳx41ⲻ5A._41 node, StringBuilder context)
            {
                context.Append("%");
                _4Transcriber.Instance.Transcribe(node._4_1, context);
                _1Transcriber.Instance.Transcribe(node._1_1, context);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx41ⲻ5A._42 node, StringBuilder context)
            {
                context.Append("%");
                _4Transcriber.Instance.Transcribe(node._4_1, context);
                _2Transcriber.Instance.Transcribe(node._2_1, context);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx41ⲻ5A._43 node, StringBuilder context)
            {
                context.Append("%");
                _4Transcriber.Instance.Transcribe(node._4_1, context);
                _3Transcriber.Instance.Transcribe(node._3_1, context);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx41ⲻ5A._44 node, StringBuilder context)
            {
                context.Append("%");
                _4Transcriber.Instance.Transcribe(node._4_1, context);
                _4Transcriber.Instance.Transcribe(node._4_1, context);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx41ⲻ5A._45 node, StringBuilder context)
            {
                context.Append("%");
                _4Transcriber.Instance.Transcribe(node._4_1, context);
                _5Transcriber.Instance.Transcribe(node._5_1, context);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx41ⲻ5A._46 node, StringBuilder context)
            {
                context.Append("%");
                _4Transcriber.Instance.Transcribe(node._4_1, context);
                _6Transcriber.Instance.Transcribe(node._6_1, context);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx41ⲻ5A._47 node, StringBuilder context)
            {
                context.Append("%");
                _4Transcriber.Instance.Transcribe(node._4_1, context);
                _7Transcriber.Instance.Transcribe(node._7_1, context);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx41ⲻ5A._48 node, StringBuilder context)
            {
                context.Append("%");
                _4Transcriber.Instance.Transcribe(node._4_1, context);
                _8Transcriber.Instance.Transcribe(node._8_1, context);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx41ⲻ5A._49 node, StringBuilder context)
            {
                context.Append("%");
                _4Transcriber.Instance.Transcribe(node._4_1, context);
                _9Transcriber.Instance.Transcribe(node._9_1, context);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx41ⲻ5A._4A node, StringBuilder context)
            {
                context.Append("%");
                _4Transcriber.Instance.Transcribe(node._4_1, context);
                _ATranscriber.Instance.Transcribe(node._A_1, context);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx41ⲻ5A._4B node, StringBuilder context)
            {
                context.Append("%");
                _4Transcriber.Instance.Transcribe(node._4_1, context);
                _BTranscriber.Instance.Transcribe(node._B_1, context);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx41ⲻ5A._4C node, StringBuilder context)
            {
                context.Append("%");
                _4Transcriber.Instance.Transcribe(node._4_1, context);
                _CTranscriber.Instance.Transcribe(node._C_1, context);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx41ⲻ5A._4D node, StringBuilder context)
            {
                context.Append("%");
                _4Transcriber.Instance.Transcribe(node._4_1, context);
                _DTranscriber.Instance.Transcribe(node._D_1, context);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx41ⲻ5A._4E node, StringBuilder context)
            {
                context.Append("%");
                _4Transcriber.Instance.Transcribe(node._4_1, context);
                _ETranscriber.Instance.Transcribe(node._E_1, context);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx41ⲻ5A._4F node, StringBuilder context)
            {
                context.Append("%");
                _4Transcriber.Instance.Transcribe(node._4_1, context);
                _FTranscriber.Instance.Transcribe(node._F_1, context);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx41ⲻ5A._50 node, StringBuilder context)
            {
                context.Append("%");
                _5Transcriber.Instance.Transcribe(node._5_1, context);
                _0Transcriber.Instance.Transcribe(node._0_1, context);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx41ⲻ5A._51 node, StringBuilder context)
            {
                context.Append("%");
                _5Transcriber.Instance.Transcribe(node._5_1, context);
                _1Transcriber.Instance.Transcribe(node._1_1, context);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx41ⲻ5A._52 node, StringBuilder context)
            {
                context.Append("%");
                _5Transcriber.Instance.Transcribe(node._5_1, context);
                _2Transcriber.Instance.Transcribe(node._2_1, context);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx41ⲻ5A._53 node, StringBuilder context)
            {
                context.Append("%");
                _5Transcriber.Instance.Transcribe(node._5_1, context);
                _3Transcriber.Instance.Transcribe(node._3_1, context);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx41ⲻ5A._54 node, StringBuilder context)
            {
                context.Append("%");
                _5Transcriber.Instance.Transcribe(node._5_1, context);
                _4Transcriber.Instance.Transcribe(node._4_1, context);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx41ⲻ5A._55 node, StringBuilder context)
            {
                context.Append("%");
                _5Transcriber.Instance.Transcribe(node._5_1, context);
                _5Transcriber.Instance.Transcribe(node._5_1, context);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx41ⲻ5A._56 node, StringBuilder context)
            {
                context.Append("%");
                _5Transcriber.Instance.Transcribe(node._5_1, context);
                _6Transcriber.Instance.Transcribe(node._6_1, context);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx41ⲻ5A._57 node, StringBuilder context)
            {
                context.Append("%");
                _5Transcriber.Instance.Transcribe(node._5_1, context);
                _7Transcriber.Instance.Transcribe(node._7_1, context);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx41ⲻ5A._58 node, StringBuilder context)
            {
                context.Append("%");
                _5Transcriber.Instance.Transcribe(node._5_1, context);
                _8Transcriber.Instance.Transcribe(node._8_1, context);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx41ⲻ5A._59 node, StringBuilder context)
            {
                context.Append("%");
                _5Transcriber.Instance.Transcribe(node._5_1, context);
                _9Transcriber.Instance.Transcribe(node._9_1, context);

                return default;
            }

            protected internal override Void Accept(Inners._Ⰳx41ⲻ5A._5A node, StringBuilder context)
            {
                context.Append("%");
                _5Transcriber.Instance.Transcribe(node._5_1, context);
                _ATranscriber.Instance.Transcribe(node._A_1, context);

                return default;
            }
        }
    }
}
