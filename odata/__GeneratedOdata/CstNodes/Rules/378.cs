namespace __GeneratedOdata.CstNodes.Rules
{
    public abstract class _SEMI
    {
        private _SEMI()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_SEMI node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_SEMI._ʺx3Bʺ node, TContext context);
            protected internal abstract TResult Accept(_SEMI._ʺx25x33x42ʺ node, TContext context);
        }
        
        public sealed class _ʺx3Bʺ : _SEMI
        {
            public _ʺx3Bʺ(__GeneratedOdata.CstNodes.Inners._ʺx3Bʺ _ʺx3Bʺ_1)
            {
                this._ʺx3Bʺ_1 = _ʺx3Bʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx3Bʺ _ʺx3Bʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx25x33x42ʺ : _SEMI
        {
            public _ʺx25x33x42ʺ(__GeneratedOdata.CstNodes.Inners._ʺx25x33x42ʺ _ʺx25x33x42ʺ_1)
            {
                this._ʺx25x33x42ʺ_1 = _ʺx25x33x42ʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx25x33x42ʺ _ʺx25x33x42ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
