namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;

    public sealed class RuleListTranscriber : ITranscriber<_rulelist>
    {
        private RuleListTranscriber()
        {
        }

        public static RuleListTranscriber Instance { get; } = new RuleListTranscriber();

        public void Transcribe(_rulelist value, StringBuilder builder)
        {

        }
    }
}
