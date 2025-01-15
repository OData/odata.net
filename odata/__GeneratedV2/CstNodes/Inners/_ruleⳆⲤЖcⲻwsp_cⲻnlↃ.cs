namespace __GeneratedV2.CstNodes.Inners
{
    public abstract class _ruleⳆⲤЖcⲻwsp_cⲻnlↃ
    {
        private _ruleⳆⲤЖcⲻwsp_cⲻnlↃ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ruleⳆⲤЖcⲻwsp_cⲻnlↃ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ruleⳆⲤЖcⲻwsp_cⲻnlↃ._rule node, TContext context);
            protected internal abstract TResult Accept(_ruleⳆⲤЖcⲻwsp_cⲻnlↃ._ⲤЖcⲻwsp_cⲻnlↃ node, TContext context);
        }
        
        public sealed class _rule : _ruleⳆⲤЖcⲻwsp_cⲻnlↃ
        {
            public _rule(__GeneratedV2.CstNodes.Rules._rule _rule_1)
            {
                this._rule_1 = _rule_1;
            }
            
            public __GeneratedV2.CstNodes.Rules._rule _rule_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ⲤЖcⲻwsp_cⲻnlↃ : _ruleⳆⲤЖcⲻwsp_cⲻnlↃ
        {
            public _ⲤЖcⲻwsp_cⲻnlↃ(__GeneratedV2.CstNodes.Inners._ⲤЖcⲻwsp_cⲻnlↃ _ⲤЖcⲻwsp_cⲻnlↃ_1)
            {
                this._ⲤЖcⲻwsp_cⲻnlↃ_1 = _ⲤЖcⲻwsp_cⲻnlↃ_1;
            }
            
            public __GeneratedV2.CstNodes.Inners._ⲤЖcⲻwsp_cⲻnlↃ _ⲤЖcⲻwsp_cⲻnlↃ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
