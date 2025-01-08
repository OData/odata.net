namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class _concatenationTranscriber : ITranscriber<_concatenation>
    {
        private _concatenationTranscriber()
        {
        }

        public static _concatenationTranscriber Instance { get; } = new _concatenationTranscriber();

        public void Transcribe(_concatenation value, StringBuilder builder)
        {
        }
    }
}
