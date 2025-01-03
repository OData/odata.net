namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;
    using GeneratorV3.SpikeTranscribers.Inners;

    public sealed class VcharTranscriber : ITranscriber<_VCHAR>
    {
        private VcharTranscriber()
        {
        }

        public static VcharTranscriber Instance { get; } = new VcharTranscriber();

        public void Transcribe(_VCHAR value, StringBuilder builder)
        {
            _Ⰳx21ⲻ7ETranscriber.Instance.Transcribe(value._Ⰳx21ⲻ7E_1, builder);
        }
    }
}
