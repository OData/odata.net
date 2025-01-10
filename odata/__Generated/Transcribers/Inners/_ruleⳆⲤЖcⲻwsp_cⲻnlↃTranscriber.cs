namespace __Generated.Trancsribers.Inners
{
    public sealed class _ruleⳆⲤЖcⲻwsp_cⲻnlↃTranscriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Inners._ruleⳆⲤЖcⲻwsp_cⲻnlↃ>
    {
        private _ruleⳆⲤЖcⲻwsp_cⲻnlↃTranscriber()
        {
        }
        
        public static _ruleⳆⲤЖcⲻwsp_cⲻnlↃTranscriber Instance { get; } = new _ruleⳆⲤЖcⲻwsp_cⲻnlↃTranscriber();
        
        public void Transcribe(__Generated.CstNodes.Inners._ruleⳆⲤЖcⲻwsp_cⲻnlↃ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __Generated.CstNodes.Inners._ruleⳆⲤЖcⲻwsp_cⲻnlↃ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Inners._ruleⳆⲤЖcⲻwsp_cⲻnlↃ._rule node, System.Text.StringBuilder context)
            {
                __Generated.Trancsribers.Rules._ruleTranscriber.Instance.Transcribe(node._rule_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Inners._ruleⳆⲤЖcⲻwsp_cⲻnlↃ._ⲤЖcⲻwsp_cⲻnlↃ node, System.Text.StringBuilder context)
            {
                __Generated.Trancsribers.Inners._ⲤЖcⲻwsp_cⲻnlↃTranscriber.Instance.Transcribe(node._ⲤЖcⲻwsp_cⲻnlↃ_1, context);

return default;
            }
        }
    }
    
}
