namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;
    using GeneratorV3.SpikeTranscribers.Rules;

    public sealed class _ʺx2Eʺ_1ЖHEXDIGTranscriber : ITranscriber<Inners._ʺx2Eʺ_1ЖHEXDIG>
    {
        private _ʺx2Eʺ_1ЖHEXDIGTranscriber()
        {
        }

        public static _ʺx2Eʺ_1ЖHEXDIGTranscriber Instance { get; } = new _ʺx2Eʺ_1ЖHEXDIGTranscriber();

        public void Transcribe(Inners._ʺx2Eʺ_1ЖHEXDIG value, StringBuilder builder)
        {
            _ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_1, builder);
            foreach (var _HEXDIG in value._HEXDIG_1)
            {
                HexDigTranscriber.Instance.Transcribe(_HEXDIG, builder);
            }
        }
    }
}
