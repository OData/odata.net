namespace __GeneratedOdata.CstNodes.Rules
{
    public abstract class _CLOSE
    {
        private _CLOSE()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_CLOSE node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_CLOSE._ʺx29ʺ node, TContext context);
            protected internal abstract TResult Accept(_CLOSE._ʺx25x32x39ʺ node, TContext context);
        }
        
        public sealed class _ʺx29ʺ : _CLOSE
        {
            public _ʺx29ʺ(__GeneratedOdata.CstNodes.Inners._ʺx29ʺ _ʺx29ʺ_1)
            {
                this._ʺx29ʺ_1 = _ʺx29ʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx29ʺ _ʺx29ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx25x32x39ʺ : _CLOSE
        {
            public _ʺx25x32x39ʺ(__GeneratedOdata.CstNodes.Inners._ʺx25x32x39ʺ _ʺx25x32x39ʺ_1)
            {
                this._ʺx25x32x39ʺ_1 = _ʺx25x32x39ʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx25x32x39ʺ _ʺx25x32x39ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}