namespace __GeneratedOdata.CstNodes.Rules
{
    public abstract class _COLON
    {
        private _COLON()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_COLON node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_COLON._ʺx3Aʺ node, TContext context);
            protected internal abstract TResult Accept(_COLON._ʺx25x33x41ʺ node, TContext context);
        }
        
        public sealed class _ʺx3Aʺ : _COLON
        {
            public _ʺx3Aʺ(__GeneratedOdata.CstNodes.Inners._ʺx3Aʺ _ʺx3Aʺ_1)
            {
                this._ʺx3Aʺ_1 = _ʺx3Aʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx3Aʺ _ʺx3Aʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx25x33x41ʺ : _COLON
        {
            public _ʺx25x33x41ʺ(__GeneratedOdata.CstNodes.Inners._ʺx25x33x41ʺ _ʺx25x33x41ʺ_1)
            {
                this._ʺx25x33x41ʺ_1 = _ʺx25x33x41ʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx25x33x41ʺ _ʺx25x33x41ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
