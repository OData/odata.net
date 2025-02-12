namespace __GeneratedOdataV2.CstNodes.Inners
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
            private _ʺx7Bʺ()
            {
                this._ʺx7Bʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx7Bʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx7Bʺ _ʺx7Bʺ_1 { get; }
            public static _ʺx7Bʺ Instance { get; } = new _ʺx7Bʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx25x37x42ʺ : _ʺx7BʺⳆʺx25x37x42ʺ
        {
            private _ʺx25x37x42ʺ()
            {
                this._ʺx25x37x42ʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx25x37x42ʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx25x37x42ʺ _ʺx25x37x42ʺ_1 { get; }
            public static _ʺx25x37x42ʺ Instance { get; } = new _ʺx25x37x42ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
