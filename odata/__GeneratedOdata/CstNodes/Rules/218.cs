namespace __GeneratedOdata.CstNodes.Rules
{
    public abstract class _escape
    {
        private _escape()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_escape node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_escape._ʺx5Cʺ node, TContext context);
            protected internal abstract TResult Accept(_escape._ʺx25x35x43ʺ node, TContext context);
        }
        
        public sealed class _ʺx5Cʺ : _escape
        {
            public _ʺx5Cʺ(__GeneratedOdata.CstNodes.Inners._ʺx5Cʺ _ʺx5Cʺ_1)
            {
                this._ʺx5Cʺ_1 = _ʺx5Cʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx5Cʺ _ʺx5Cʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx25x35x43ʺ : _escape
        {
            public _ʺx25x35x43ʺ(__GeneratedOdata.CstNodes.Inners._ʺx25x35x43ʺ _ʺx25x35x43ʺ_1)
            {
                this._ʺx25x35x43ʺ_1 = _ʺx25x35x43ʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx25x35x43ʺ _ʺx25x35x43ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
