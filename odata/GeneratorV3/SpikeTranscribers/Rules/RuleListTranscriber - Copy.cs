namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class RuleList2Transcriber : ITranscriber<_rulelist>
    {
        private RuleList2Transcriber()
        {
        }

        public static RuleList2Transcriber Instance { get; } = new RuleList2Transcriber();

        public void Transcribe(_rulelist value, StringBuilder builder)
        {

        }
    }
}
