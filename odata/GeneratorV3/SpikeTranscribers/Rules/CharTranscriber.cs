namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;
    using GeneratorV3.SpikeTranscribers.Inners;

    public sealed class CharTranscriber : ITranscriber<_CHAR>
    {
        private CharTranscriber()
        {
        }

        public static CharTranscriber Instance { get; } = new CharTranscriber();

        public void Transcribe(_CHAR value, StringBuilder builder)
        {
            _Ⰳx01ⲻ7FTranscriber.Instance.Transcribe(value._Ⰳx01ⲻ7F_1, builder);
        }
    }
}
