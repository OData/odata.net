namespace __GeneratedOdataV2.CstNodes.Inners
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
            public _ʺx5Dʺ(__GeneratedOdataV2.CstNodes.Inners._ʺx5Dʺ _ʺx5Dʺ_1)
            {
                this._ʺx5Dʺ_1 = _ʺx5Dʺ_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx5Dʺ _ʺx5Dʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx25x35x44ʺ : _ʺx5DʺⳆʺx25x35x44ʺ
        {
            public _ʺx25x35x44ʺ(__GeneratedOdataV2.CstNodes.Inners._ʺx25x35x44ʺ _ʺx25x35x44ʺ_1)
            {
                this._ʺx25x35x44ʺ_1 = _ʺx25x35x44ʺ_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx25x35x44ʺ _ʺx25x35x44ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
