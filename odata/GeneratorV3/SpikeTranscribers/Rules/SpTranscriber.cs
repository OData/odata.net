namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class SpTranscriber : ITranscriber<_SP>
    {
        private SpTranscriber()
        {
        }

        public static SpTranscriber Instance { get; } = new SpTranscriber();

        public void Transcribe(_SP value, StringBuilder builder)
        {
            builder.Append((char)0x20);
        }
    }
}
