namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class LfTranscriber : ITranscriber<_LF>
    {
        private LfTranscriber()
        {
        }

        public static LfTranscriber Instance { get; } = new LfTranscriber();

        public void Transcribe(_LF value, StringBuilder builder)
        {
            builder.Append((char)0x0A);
        }
    }
}
