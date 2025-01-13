namespace __GeneratedOdata.CstNodes.Rules
{
    public abstract class _quotationⲻmark
    {
        private _quotationⲻmark()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_quotationⲻmark node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_quotationⲻmark._DQUOTE node, TContext context);
            protected internal abstract TResult Accept(_quotationⲻmark._ʺx25x32x32ʺ node, TContext context);
        }
        
        public sealed class _DQUOTE : _quotationⲻmark
        {
            public _DQUOTE(__GeneratedOdata.CstNodes.Rules._DQUOTE _DQUOTE_1)
            {
                this._DQUOTE_1 = _DQUOTE_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._DQUOTE _DQUOTE_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx25x32x32ʺ : _quotationⲻmark
        {
            public _ʺx25x32x32ʺ(__GeneratedOdata.CstNodes.Inners._ʺx25x32x32ʺ _ʺx25x32x32ʺ_1)
            {
                this._ʺx25x32x32ʺ_1 = _ʺx25x32x32ʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx25x32x32ʺ _ʺx25x32x32ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
