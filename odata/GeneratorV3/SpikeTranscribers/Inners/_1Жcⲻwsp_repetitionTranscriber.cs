namespace GeneratorV3.SpikeTranscribers.Inners
{
    using System.Text;

    using GeneratorV3.Abnf;
    using GeneratorV3.SpikeTranscribers.Rules;

    public sealed class _1Жcⲻwsp_repetitionTranscriber : ITranscriber<Inners._1Жcⲻwsp_repetition>
    {
        private _1Жcⲻwsp_repetitionTranscriber()
        {
        }

        public static _1Жcⲻwsp_repetitionTranscriber Instance { get; } = new _1Жcⲻwsp_repetitionTranscriber();

        public void Transcribe(Inners._1Жcⲻwsp_repetition value, StringBuilder builder)
        {
            foreach (var _cⲻwsp in value._cⲻwsp_1)
            {
                _cⲻwspTranscriber.Instance.Transcribe(_cⲻwsp, builder);
            }

            _repetitionTranscriber.Instance.Transcribe(value._repetition_1, builder);
        }
    }
}
