namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;
    using static GeneratorV3.Abnf.Inners;

    public sealed class _ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃTranscriber : ITranscriber<_ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ>
    {
        private _ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃTranscriber()
        {
        }

        public static _ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃTranscriber Instance { get; } = new _ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃTranscriber();

        public void Transcribe(_ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ value, StringBuilder builder)
        {
            _ruleⳆⲤЖcⲻwsp_cⲻnlↃTranscriber.Instance.Transcribe(value._ruleⳆⲤЖcⲻwsp_cⲻnlↃ_1, builder);
        }
    }
}
