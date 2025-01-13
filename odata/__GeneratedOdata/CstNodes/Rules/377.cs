namespace __GeneratedOdata.CstNodes.Rules
{
    public abstract class _SIGN
    {
        private _SIGN()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_SIGN node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_SIGN._ʺx2Bʺ node, TContext context);
            protected internal abstract TResult Accept(_SIGN._ʺx25x32x42ʺ node, TContext context);
            protected internal abstract TResult Accept(_SIGN._ʺx2Dʺ node, TContext context);
        }
        
        public sealed class _ʺx2Bʺ : _SIGN
        {
            public _ʺx2Bʺ(__GeneratedOdata.CstNodes.Inners._ʺx2Bʺ _ʺx2Bʺ_1)
            {
                this._ʺx2Bʺ_1 = _ʺx2Bʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx2Bʺ _ʺx2Bʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx25x32x42ʺ : _SIGN
        {
            public _ʺx25x32x42ʺ(__GeneratedOdata.CstNodes.Inners._ʺx25x32x42ʺ _ʺx25x32x42ʺ_1)
            {
                this._ʺx25x32x42ʺ_1 = _ʺx25x32x42ʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx25x32x42ʺ _ʺx25x32x42ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx2Dʺ : _SIGN
        {
            public _ʺx2Dʺ(__GeneratedOdata.CstNodes.Inners._ʺx2Dʺ _ʺx2Dʺ_1)
            {
                this._ʺx2Dʺ_1 = _ʺx2Dʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx2Dʺ _ʺx2Dʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
