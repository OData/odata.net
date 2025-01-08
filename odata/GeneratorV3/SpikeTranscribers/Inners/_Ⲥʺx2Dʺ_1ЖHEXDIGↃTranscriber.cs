namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _Ⲥʺx2Dʺ_1ЖHEXDIGↃTranscriber : ITranscriber<Inners._Ⲥʺx2Dʺ_1ЖHEXDIGↃ>
    {
        private _Ⲥʺx2Dʺ_1ЖHEXDIGↃTranscriber()
        {
        }

        public static _Ⲥʺx2Dʺ_1ЖHEXDIGↃTranscriber Instance { get; } = new _Ⲥʺx2Dʺ_1ЖHEXDIGↃTranscriber();

        public void Transcribe(Inners._Ⲥʺx2Dʺ_1ЖHEXDIGↃ value, StringBuilder builder)
        {
            _ʺx2Dʺ_1ЖHEXDIGTranscriber.Instance.Transcribe(value._ʺx2Dʺ_1ЖHEXDIG_1, builder);
        }
    }
}
