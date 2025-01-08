namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;
    using GeneratorV3.SpikeTranscribers.Inners;

    public sealed class _optionTranscriber : ITranscriber<_option>
    {
        private _optionTranscriber()
        {
        }

        public static _optionTranscriber Instance { get; } = new _optionTranscriber();

        public void Transcribe(_option value, StringBuilder builder)
        {
            _ʺx5BʺTranscriber.Instance.Transcribe(value._ʺx5Bʺ_1, builder);
            foreach (var _cⲻwsp in value._cⲻwsp_1)
            {
                _cⲻwspTranscriber.Instance.Transcribe(_cⲻwsp, builder);
            }

            _alternationTranscriber.Instance.Transcribe(value._alternation_1, builder);

            foreach (var _cⲻwsp in value._cⲻwsp_2)
            {
                _cⲻwspTranscriber.Instance.Transcribe(_cⲻwsp, builder);
            }

            _ʺx5DʺTranscriber.Instance.Transcribe(value._ʺx5Dʺ_1, builder);
        }
    }
}
