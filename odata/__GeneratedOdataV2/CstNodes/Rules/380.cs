namespace __GeneratedOdataV2.CstNodes.Rules
{
    public abstract class _SQUOTE
    {
        private _SQUOTE()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_SQUOTE node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_SQUOTE._ʺx27ʺ node, TContext context);
            protected internal abstract TResult Accept(_SQUOTE._ʺx25x32x37ʺ node, TContext context);
        }
        
        public sealed class _ʺx27ʺ : _SQUOTE
        {
            public _ʺx27ʺ(__GeneratedOdataV2.CstNodes.Inners._ʺx27ʺ _ʺx27ʺ_1)
            {
                this._ʺx27ʺ_1 = _ʺx27ʺ_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx27ʺ _ʺx27ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx25x32x37ʺ : _SQUOTE
        {
            public _ʺx25x32x37ʺ(__GeneratedOdataV2.CstNodes.Inners._ʺx25x32x37ʺ _ʺx25x32x37ʺ_1)
            {
                this._ʺx25x32x37ʺ_1 = _ʺx25x32x37ʺ_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx25x32x37ʺ _ʺx25x32x37ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
