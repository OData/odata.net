namespace __GeneratedOdata.CstNodes.Rules
{
    public abstract class _COMMA
    {
        private _COMMA()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_COMMA node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_COMMA._ʺx2Cʺ node, TContext context);
            protected internal abstract TResult Accept(_COMMA._ʺx25x32x43ʺ node, TContext context);
        }
        
        public sealed class _ʺx2Cʺ : _COMMA
        {
            public _ʺx2Cʺ(__GeneratedOdata.CstNodes.Inners._ʺx2Cʺ _ʺx2Cʺ_1)
            {
                this._ʺx2Cʺ_1 = _ʺx2Cʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx2Cʺ _ʺx2Cʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx25x32x43ʺ : _COMMA
        {
            public _ʺx25x32x43ʺ(__GeneratedOdata.CstNodes.Inners._ʺx25x32x43ʺ _ʺx25x32x43ʺ_1)
            {
                this._ʺx25x32x43ʺ_1 = _ʺx25x32x43ʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx25x32x43ʺ _ʺx25x32x43ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
