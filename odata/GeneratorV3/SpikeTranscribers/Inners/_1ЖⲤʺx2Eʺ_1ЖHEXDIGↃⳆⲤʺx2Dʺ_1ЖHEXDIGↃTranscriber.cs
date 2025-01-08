namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;
    using Root;

    public sealed class _1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃTranscriber : ITranscriber<Inners._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ>
    {
        private _1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃTranscriber()
        {
        }

        public static _1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃTranscriber Instance { get; } = new _1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃTranscriber();

        public void Transcribe(Inners._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ value, StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }

        private sealed class Visitor : Inners._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ.Visitor<Root.Void, StringBuilder>
        {
            private Visitor()
            {
            }

            public static Visitor Instance { get; } = new Visitor();

            protected internal override Void Accept(Inners._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃ node, StringBuilder context)
            {
                foreach (var _Ⲥʺx2Eʺ_1ЖHEXDIGↃ in node._Ⲥʺx2Eʺ_1ЖHEXDIGↃ_1)
                {
                    _Ⲥʺx2Eʺ_1ЖHEXDIGↃTranscriber.Instance.Transcribe(_Ⲥʺx2Eʺ_1ЖHEXDIGↃ, context);
                }

                return default;
            }

            protected internal override Void Accept(Inners._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ._Ⲥʺx2Dʺ_1ЖHEXDIGↃ node, StringBuilder context)
            {
                _Ⲥʺx2Dʺ_1ЖHEXDIGↃTranscriber.Instance.Transcribe(node._Ⲥʺx2Dʺ_1ЖHEXDIGↃ_1, context);

                return default;
            }
        }
    }
}
