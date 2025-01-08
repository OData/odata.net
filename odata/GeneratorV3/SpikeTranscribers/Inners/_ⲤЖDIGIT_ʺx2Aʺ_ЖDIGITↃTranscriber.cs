namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃTranscriber : ITranscriber<Inners._ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ>
    {
        private _ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃTranscriber()
        {
        }

        public static _ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃTranscriber Instance { get; } = new _ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃTranscriber();

        public void Transcribe(Inners._ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ value, StringBuilder builder)
        {
            _ЖDIGIT_ʺx2Aʺ_ЖDIGITTranscriber.Instance.Transcribe(value._ЖDIGIT_ʺx2Aʺ_ЖDIGIT_1, builder);
        }
    }
}
