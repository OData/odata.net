namespace __GeneratedOdataV4.CstNodes.Rules
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
            private _ʺx5Cʺ()
            {
                this._ʺx5Cʺ_1 = __GeneratedOdataV4.CstNodes.Inners._ʺx5Cʺ.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx5Cʺ _ʺx5Cʺ_1 { get; }
            public static _ʺx5Cʺ Instance { get; } = new _ʺx5Cʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx25x35x43ʺ : _escape
        {
            private _ʺx25x35x43ʺ()
            {
                this._ʺx25x35x43ʺ_1 = __GeneratedOdataV4.CstNodes.Inners._ʺx25x35x43ʺ.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx25x35x43ʺ _ʺx25x35x43ʺ_1 { get; }
            public static _ʺx25x35x43ʺ Instance { get; } = new _ʺx25x35x43ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
