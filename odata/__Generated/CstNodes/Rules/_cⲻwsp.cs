namespace __Generated.CstNodes.Rules
{
    public abstract class _cⲻwsp
    {
        private _cⲻwsp()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_cⲻwsp node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_cⲻwsp._WSP node, TContext context);
            protected internal abstract TResult Accept(_cⲻwsp._Ⲥcⲻnl_WSPↃ node, TContext context);
        }
        
        public sealed class _WSP : _cⲻwsp
        {
            public _WSP(__Generated.CstNodes.Rules._WSP _WSP_1)
            {
                this._WSP_1 = _WSP_1;
            }
            
            public __Generated.CstNodes.Rules._WSP _WSP_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _Ⲥcⲻnl_WSPↃ : _cⲻwsp
        {
            public _Ⲥcⲻnl_WSPↃ(__Generated.CstNodes.Inners._Ⲥcⲻnl_WSPↃ _Ⲥcⲻnl_WSPↃ_1)
            {
                this._Ⲥcⲻnl_WSPↃ_1 = _Ⲥcⲻnl_WSPↃ_1;
            }
            
            public __Generated.CstNodes.Inners._Ⲥcⲻnl_WSPↃ _Ⲥcⲻnl_WSPↃ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
