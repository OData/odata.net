namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;
    using GeneratorV3.SpikeTranscribers.Rules;

    public sealed class _Жcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationTranscriber : ITranscriber<Inners._Жcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenation>
    {
        private _Жcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationTranscriber()
        {
        }

        public static _Жcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationTranscriber Instance { get; } = new _Жcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationTranscriber();

        public void Transcribe(Inners._Жcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenation value, StringBuilder builder)
        {
            foreach (var _cⲻwsp in value._cⲻwsp_1)
            {
                _cⲻwspTranscriber.Instance.Transcribe(_cⲻwsp, builder);
            }

            _ʺx2FʺTranscriber.Instance.Transcribe(value._ʺx2Fʺ_1, builder);

            foreach (var _cⲻwsp in value._cⲻwsp_2)
            {
                _cⲻwspTranscriber.Instance.Transcribe(_cⲻwsp, builder);
            }

            _concatenationTranscriber.Instance.Transcribe(value._concatenation_1, builder);
        }
    }
}
