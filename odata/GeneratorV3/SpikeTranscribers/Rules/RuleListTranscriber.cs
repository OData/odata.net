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
            foreach (var _ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ in value._ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ_1)
            {
                _ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃTranscriber.Instance.Transcribe(_ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ, builder);
            }
        }
    }
}
