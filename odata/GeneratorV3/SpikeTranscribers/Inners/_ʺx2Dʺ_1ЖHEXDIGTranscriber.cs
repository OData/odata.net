namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;
    using GeneratorV3.SpikeTranscribers.Rules;

    public sealed class _ʺx2Dʺ_1ЖHEXDIGTranscriber : ITranscriber<Inners._ʺx2Dʺ_1ЖHEXDIG>
    {
        private _ʺx2Dʺ_1ЖHEXDIGTranscriber()
        {
        }

        public static _ʺx2Dʺ_1ЖHEXDIGTranscriber Instance { get; } = new _ʺx2Dʺ_1ЖHEXDIGTranscriber();

        public void Transcribe(Inners._ʺx2Dʺ_1ЖHEXDIG value, StringBuilder builder)
        {
            _ʺx2DʺTranscriber.Instance.Transcribe(value._ʺx2Dʺ_1, builder);

            foreach (var _HEXDIG in value._HEXDIG_1)
            {
                HexDigTranscriber.Instance.Transcribe(_HEXDIG, builder);
            }
        }
    }
}
