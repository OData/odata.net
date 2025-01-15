namespace __GeneratedV2.CstNodes.Rules
{
    public abstract class _BIT
    {
        private _BIT()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_BIT node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_BIT._ʺx30ʺ node, TContext context);
            protected internal abstract TResult Accept(_BIT._ʺx31ʺ node, TContext context);
        }
        
        public sealed class _ʺx30ʺ : _BIT
        {
            public _ʺx30ʺ(__GeneratedV2.CstNodes.Inners._ʺx30ʺ _ʺx30ʺ_1)
            {
                this._ʺx30ʺ_1 = _ʺx30ʺ_1;
            }
            
            public __GeneratedV2.CstNodes.Inners._ʺx30ʺ _ʺx30ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx31ʺ : _BIT
        {
            public _ʺx31ʺ(__GeneratedV2.CstNodes.Inners._ʺx31ʺ _ʺx31ʺ_1)
            {
                this._ʺx31ʺ_1 = _ʺx31ʺ_1;
            }
            
            public __GeneratedV2.CstNodes.Inners._ʺx31ʺ _ʺx31ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
