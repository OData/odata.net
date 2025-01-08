namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _Ⲥʺx2Eʺ_1ЖHEXDIGↃTranscriber : ITranscriber<Inners._Ⲥʺx2Eʺ_1ЖHEXDIGↃ>
    {
        private _Ⲥʺx2Eʺ_1ЖHEXDIGↃTranscriber()
        {
        }

        public static _Ⲥʺx2Eʺ_1ЖHEXDIGↃTranscriber Instance { get; } = new _Ⲥʺx2Eʺ_1ЖHEXDIGↃTranscriber();

        public void Transcribe(Inners._Ⲥʺx2Eʺ_1ЖHEXDIGↃ value, StringBuilder builder)
        {
            _ʺx2Eʺ_1ЖHEXDIGTranscriber.Instance.Transcribe(value._ʺx2Eʺ_1ЖHEXDIG_1, builder);
        }
    }
}
