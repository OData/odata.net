namespace __Generated.CstNodes.Rules
{
    public abstract class _repeat
    {
        private _repeat()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_repeat node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_repeat._1ЖDIGIT node, TContext context);
            protected internal abstract TResult Accept(_repeat._ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ node, TContext context);
        }
        
        public sealed class _1ЖDIGIT : _repeat
        {
            public _1ЖDIGIT(System.Collections.Generic.IEnumerable<__Generated.CstNodes.Rules._DIGIT> _DIGIT_1)
            {
                this._DIGIT_1 = _DIGIT_1;
            }
            
            public System.Collections.Generic.IEnumerable<__Generated.CstNodes.Rules._DIGIT> _DIGIT_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ : _repeat
        {
            public _ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ(__Generated.CstNodes.Inners._ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ _ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ_1)
            {
                this._ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ_1 = _ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ_1;
            }
            
            public __Generated.CstNodes.Inners._ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ _ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
