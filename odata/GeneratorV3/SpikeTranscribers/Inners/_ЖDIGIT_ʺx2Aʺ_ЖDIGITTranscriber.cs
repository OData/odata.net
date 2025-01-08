namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;
    using GeneratorV3.SpikeTranscribers.Rules;

    public sealed class _ЖDIGIT_ʺx2Aʺ_ЖDIGITTranscriber : ITranscriber<Inners._ЖDIGIT_ʺx2Aʺ_ЖDIGIT>
    {
        private _ЖDIGIT_ʺx2Aʺ_ЖDIGITTranscriber()
        {
        }

        public static _ЖDIGIT_ʺx2Aʺ_ЖDIGITTranscriber Instance { get; } = new _ЖDIGIT_ʺx2Aʺ_ЖDIGITTranscriber();

        public void Transcribe(Inners._ЖDIGIT_ʺx2Aʺ_ЖDIGIT value, StringBuilder builder)
        {
            foreach (var _DIGIT in value._DIGIT_1)
            {
                DigitTranscriber.Instance.Transcribe(_DIGIT, builder);
            }

            _ʺx2AʺTranscriber.Instance.Transcribe(value._ʺx2Aʺ_1, builder);

            foreach (var _DIGIT in value._DIGIT_2)
            {
                DigitTranscriber.Instance.Transcribe(_DIGIT, builder);
            }
        }
    }
}
