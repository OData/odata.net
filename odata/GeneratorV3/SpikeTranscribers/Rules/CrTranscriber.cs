namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class CrTranscriber : ITranscriber<_CR>
    {
        private CrTranscriber()
        {
        }

        public static CrTranscriber Instance { get; } = new CrTranscriber();

        public void Transcribe(_CR value, StringBuilder builder)
        {
            builder.Append((char)0x0D);
        }
    }
}
