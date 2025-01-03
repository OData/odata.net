namespace GeneratorV3.SpikeTranscribers.Rules
{
    using GeneratorV3.Abnf;
    using System.Text;

    public sealed class CrLfTranscriber : ITranscriber<_CRLF>
    {
        private CrLfTranscriber()
        {
        }

        public static CrLfTranscriber Instance { get; } = new CrLfTranscriber();

        public void Transcribe(_CRLF value, StringBuilder builder)
        {
            CrTranscriber.Instance.Transcribe(value._CR_1, builder);
            LfTranscriber.Instance.Transcribe(value._LF_1, builder);
        }
    }
}
