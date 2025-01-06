namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;
    using Root;
    using static GeneratorV3.Abnf.Inners;

    public sealed class _ruleⳆⲤЖcⲻwsp_cⲻnlↃTranscriber : ITranscriber<_ruleⳆⲤЖcⲻwsp_cⲻnlↃ>
    {
        private _ruleⳆⲤЖcⲻwsp_cⲻnlↃTranscriber()
        {
        }

        public static _ruleⳆⲤЖcⲻwsp_cⲻnlↃTranscriber Instance { get; } = new _ruleⳆⲤЖcⲻwsp_cⲻnlↃTranscriber();

        public void Transcribe(_ruleⳆⲤЖcⲻwsp_cⲻnlↃ value, StringBuilder builder)
        {

        }

        private sealed class Visitor : _ruleⳆⲤЖcⲻwsp_cⲻnlↃ.Visitor<Void, StringBuilder>
        {
            private Visitor()
            {
            }

            public static Visitor Instance { get; } = new Visitor();
        }
    }
}
