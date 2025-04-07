namespace __GeneratedPartialV1.Realized.CstNodes.Rules
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
            private _ʺx30ʺ()
            {
                this._ʺx30ʺ_1 = __GeneratedPartialV1.Realized.CstNodes.Inners._ʺx30ʺ.Instance;
            }
            
            public __GeneratedPartialV1.Realized.CstNodes.Inners._ʺx30ʺ _ʺx30ʺ_1 { get; }
            public static _ʺx30ʺ Instance { get; } = new _ʺx30ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx31ʺ : _BIT
        {
            private _ʺx31ʺ()
            {
                this._ʺx31ʺ_1 = __GeneratedPartialV1.Realized.CstNodes.Inners._ʺx31ʺ.Instance;
            }
            
            public __GeneratedPartialV1.Realized.CstNodes.Inners._ʺx31ʺ _ʺx31ʺ_1 { get; }
            public static _ʺx31ʺ Instance { get; } = new _ʺx31ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
