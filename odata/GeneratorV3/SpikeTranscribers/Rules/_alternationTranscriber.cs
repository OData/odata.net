namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _alternationTranscriber : ITranscriber<_alternation>
    {
        private _alternationTranscriber()
        {
        }

        public static _alternationTranscriber Instance { get; } = new _alternationTranscriber();

        public void Transcribe(_alternation value, StringBuilder builder)
        {
            _concatenationTranscriber.Instance.Transcribe(value._concatenation_1, builder);
        }
    }
}
