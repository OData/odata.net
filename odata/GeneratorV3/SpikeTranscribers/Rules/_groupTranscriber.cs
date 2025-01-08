namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;
    using GeneratorV3.SpikeTranscribers.Inners;

    public sealed class _groupTranscriber : ITranscriber<_group>
    {
        private _groupTranscriber()
        {
        }

        public static _groupTranscriber Instance { get; } = new _groupTranscriber();

        public void Transcribe(_group value, StringBuilder builder)
        {
            _ʺx28ʺTranscriber.Instance.Transcribe(value._ʺx28ʺ_1, builder);

            foreach (var _cⲻwsp in value._cⲻwsp_1)
            {
                _cⲻwspTranscriber.Instance.Transcribe(_cⲻwsp, builder);
            }

            _alternationTranscriber.Instance.Transcribe(value._alternation_1, builder);

            foreach (var _cⲻwsp in value._cⲻwsp_2)
            {
                _cⲻwspTranscriber.Instance.Transcribe(_cⲻwsp, builder);
            }

            _ʺx29ʺTranscriber.Instance.Transcribe(value._ʺx29ʺ_1, builder);
        }
    }
}
