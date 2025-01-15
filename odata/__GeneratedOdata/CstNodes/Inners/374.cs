namespace __GeneratedOdata.CstNodes.Inners
{
    public abstract class _ʺx7BʺⳆʺx25x37x42ʺ
    {
        private _ʺx7BʺⳆʺx25x37x42ʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ʺx7BʺⳆʺx25x37x42ʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ʺx7BʺⳆʺx25x37x42ʺ._ʺx7Bʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx7BʺⳆʺx25x37x42ʺ._ʺx25x37x42ʺ node, TContext context);
        }
        
        public sealed class _ʺx7Bʺ : _ʺx7BʺⳆʺx25x37x42ʺ
        {
            public _ʺx7Bʺ(__GeneratedOdata.CstNodes.Inners._ʺx7Bʺ _ʺx7Bʺ_1)
            {
                this._ʺx7Bʺ_1 = _ʺx7Bʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx7Bʺ _ʺx7Bʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx25x37x42ʺ : _ʺx7BʺⳆʺx25x37x42ʺ
        {
            public _ʺx25x37x42ʺ(__GeneratedOdata.CstNodes.Inners._ʺx25x37x42ʺ _ʺx25x37x42ʺ_1)
            {
                this._ʺx25x37x42ʺ_1 = _ʺx25x37x42ʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx25x37x42ʺ _ʺx25x37x42ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
