namespace __GeneratedOdataV3.CstNodes.Inners
{
    public abstract class _ʺx5BʺⳆʺx25x35x42ʺ
    {
        private _ʺx5BʺⳆʺx25x35x42ʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ʺx5BʺⳆʺx25x35x42ʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ʺx5BʺⳆʺx25x35x42ʺ._ʺx5Bʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx5BʺⳆʺx25x35x42ʺ._ʺx25x35x42ʺ node, TContext context);
        }
        
        public sealed class _ʺx5Bʺ : _ʺx5BʺⳆʺx25x35x42ʺ
        {
            private _ʺx5Bʺ()
            {
                this._ʺx5Bʺ_1 = __GeneratedOdataV3.CstNodes.Inners._ʺx5Bʺ.Instance;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx5Bʺ _ʺx5Bʺ_1 { get; }
            public static _ʺx5Bʺ Instance { get; } = new _ʺx5Bʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx25x35x42ʺ : _ʺx5BʺⳆʺx25x35x42ʺ
        {
            private _ʺx25x35x42ʺ()
            {
                this._ʺx25x35x42ʺ_1 = __GeneratedOdataV3.CstNodes.Inners._ʺx25x35x42ʺ.Instance;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx25x35x42ʺ _ʺx25x35x42ʺ_1 { get; }
            public static _ʺx25x35x42ʺ Instance { get; } = new _ʺx25x35x42ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
