namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;
    using GeneratorV3.SpikeTranscribers.Inners;

    public sealed class _concatenationTranscriber : ITranscriber<_concatenation>
    {
        private _concatenationTranscriber()
        {
        }

        public static _concatenationTranscriber Instance { get; } = new _concatenationTranscriber();

        public void Transcribe(_concatenation value, StringBuilder builder)
        {
            _repetitionTranscriber.Instance.Transcribe(value._repetition_1, builder);

            foreach (var _Ⲥ1Жcⲻwsp_repetitionↃ in value._Ⲥ1Жcⲻwsp_repetitionↃ_1)
            {
                _Ⲥ1Жcⲻwsp_repetitionↃTranscriber.Instance.Transcribe(_Ⲥ1Жcⲻwsp_repetitionↃ, builder);
            }
        }
    }
}
