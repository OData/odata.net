namespace GeneratorV3.SpikeTranscribers.Rules
{
    using System.Text;

    using GeneratorV3.Abnf;
    using GeneratorV3.SpikeTranscribers.Inners;

    public sealed class _ruleⳆⲤЖcⲻwsp_cⲻnlↃTranscriber : ITranscriber<Inners._ruleⳆⲤЖcⲻwsp_cⲻnlↃ>
    {
        private _ruleⳆⲤЖcⲻwsp_cⲻnlↃTranscriber()
        {
        }

        public static _ruleⳆⲤЖcⲻwsp_cⲻnlↃTranscriber Instance { get; } = new _ruleⳆⲤЖcⲻwsp_cⲻnlↃTranscriber();

        public void Transcribe(Inners._ruleⳆⲤЖcⲻwsp_cⲻnlↃ value, StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }

        private sealed class Visitor : Inners._ruleⳆⲤЖcⲻwsp_cⲻnlↃ.Visitor<Root.Void, StringBuilder>
        {
            private Visitor()
            {
            }

            public static Visitor Instance { get; } = new Visitor();

            protected internal override Root.Void Accept(Inners._ruleⳆⲤЖcⲻwsp_cⲻnlↃ._rule node, StringBuilder context)
            {
                RuleTranscriber.Instance.Transcribe(node._rule_1, context);

                return default;
            }

            protected internal override Root.Void Accept(Inners._ruleⳆⲤЖcⲻwsp_cⲻnlↃ._ⲤЖcⲻwsp_cⲻnlↃ node, StringBuilder context)
            {
                _ⲤЖcⲻwsp_cⲻnlↃTranscriber.Instance.Transcribe(node._ⲤЖcⲻwsp_cⲻnlↃ_1, context);

                return default;
            }
        }
    }
}
