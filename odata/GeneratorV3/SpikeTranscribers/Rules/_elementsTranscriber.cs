namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _elementsTranscriber : ITranscriber<_elements>
    {
        private _elementsTranscriber()
        {
        }

        public static _elementsTranscriber Instance { get; } = new _elementsTranscriber();

        public void Transcribe(_elements value, StringBuilder builder)
        {
            _alternationTranscriber.Instance.Transcribe(value._alternation_1, builder);

            foreach (var _cⲻwsp in value._cⲻwsp_1)
            {
                _cⲻwspTranscriber.Instance.Transcribe(_cⲻwsp, builder);
            }
        }
    }
}
