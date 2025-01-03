namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class HtabTranscriber : ITranscriber<_HTAB>
    {
        private HtabTranscriber()
        {
        }

        public static HtabTranscriber Instance { get; } = new HtabTranscriber();

        public void Transcribe(_HTAB value, StringBuilder builder)
        {
            builder.Append((char)0x09);
        }
    }
}
