namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;
    using GeneratorV3.SpikeTranscribers.Inners;

    public sealed class OctetTranscriber : ITranscriber<_OCTET>
    {
        private OctetTranscriber()
        {
        }

        public static OctetTranscriber Instance { get; } = new OctetTranscriber();

        public void Transcribe(_OCTET value, StringBuilder builder)
        {
            _Ⰳx00ⲻFFTranscriber.Instance.Transcribe(value._Ⰳx00ⲻFF_1, builder);
        }
    }
}
