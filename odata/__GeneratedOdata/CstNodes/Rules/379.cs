namespace __GeneratedOdata.CstNodes.Rules
{
    public abstract class _STAR
    {
        private _STAR()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_STAR node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_STAR._ʺx2Aʺ node, TContext context);
            protected internal abstract TResult Accept(_STAR._ʺx25x32x41ʺ node, TContext context);
        }
        
        public sealed class _ʺx2Aʺ : _STAR
        {
            public _ʺx2Aʺ(__GeneratedOdata.CstNodes.Inners._ʺx2Aʺ _ʺx2Aʺ_1)
            {
                this._ʺx2Aʺ_1 = _ʺx2Aʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx2Aʺ _ʺx2Aʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx25x32x41ʺ : _STAR
        {
            public _ʺx25x32x41ʺ(__GeneratedOdata.CstNodes.Inners._ʺx25x32x41ʺ _ʺx25x32x41ʺ_1)
            {
                this._ʺx25x32x41ʺ_1 = _ʺx25x32x41ʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx25x32x41ʺ _ʺx25x32x41ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
