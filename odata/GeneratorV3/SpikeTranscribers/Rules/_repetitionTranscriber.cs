namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _repetitionTranscriber : ITranscriber<_repetition>
    {
        private _repetitionTranscriber()
        {
        }

        public static _repetitionTranscriber Instance { get; } = new _repetitionTranscriber();

        public void Transcribe(_repetition value, StringBuilder builder)
        {
        }
    }
}
