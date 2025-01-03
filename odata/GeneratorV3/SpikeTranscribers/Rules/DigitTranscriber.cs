namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;
    using GeneratorV3.SpikeTranscribers.Inners;

    public sealed class DigitTranscriber : ITranscriber<_DIGIT>
    {
        private DigitTranscriber()
        {
        }

        public static DigitTranscriber Instance { get; } = new DigitTranscriber();

        public void Transcribe(_DIGIT value, StringBuilder builder)
        {
            _Ⰳx30ⲻ39Transcriber.Instance.Transcribe(value._Ⰳx30ⲻ39_1, builder);
        }
    }
}
