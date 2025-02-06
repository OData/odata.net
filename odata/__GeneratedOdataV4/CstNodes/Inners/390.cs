namespace __GeneratedOdataV4.CstNodes.Inners
{
    public abstract class _ʺx5DʺⳆʺx25x35x44ʺ
    {
        private _ʺx5DʺⳆʺx25x35x44ʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ʺx5DʺⳆʺx25x35x44ʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ʺx5DʺⳆʺx25x35x44ʺ._ʺx5Dʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx5DʺⳆʺx25x35x44ʺ._ʺx25x35x44ʺ node, TContext context);
        }
        
        public sealed class _ʺx5Dʺ : _ʺx5DʺⳆʺx25x35x44ʺ
        {
            private _ʺx5Dʺ()
            {
                this._ʺx5Dʺ_1 = __GeneratedOdataV4.CstNodes.Inners._ʺx5Dʺ.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx5Dʺ _ʺx5Dʺ_1 { get; }
            public static _ʺx5Dʺ Instance { get; } = new _ʺx5Dʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx25x35x44ʺ : _ʺx5DʺⳆʺx25x35x44ʺ
        {
            private _ʺx25x35x44ʺ()
            {
                this._ʺx25x35x44ʺ_1 = __GeneratedOdataV4.CstNodes.Inners._ʺx25x35x44ʺ.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx25x35x44ʺ _ʺx25x35x44ʺ_1 { get; }
            public static _ʺx25x35x44ʺ Instance { get; } = new _ʺx25x35x44ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
