namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class DquoteTranscriber : ITranscriber<_DQUOTE>
    {
        private DquoteTranscriber()
        {
        }

        public static DquoteTranscriber Instance { get; } = new DquoteTranscriber();

        public void Transcribe(_DQUOTE value, StringBuilder builder)
        {
            builder.Append((char)0x22);
        }
    }
}
