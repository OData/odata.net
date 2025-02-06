namespace __GeneratedOdataV4.CstNodes.Rules
{
    public abstract class _SEMI
    {
        private _SEMI()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_SEMI node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_SEMI._ʺx3Bʺ node, TContext context);
            protected internal abstract TResult Accept(_SEMI._ʺx25x33x42ʺ node, TContext context);
        }
        
        public sealed class _ʺx3Bʺ : _SEMI
        {
            private _ʺx3Bʺ()
            {
                this._ʺx3Bʺ_1 = __GeneratedOdataV4.CstNodes.Inners._ʺx3Bʺ.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx3Bʺ _ʺx3Bʺ_1 { get; }
            public static _ʺx3Bʺ Instance { get; } = new _ʺx3Bʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx25x33x42ʺ : _SEMI
        {
            private _ʺx25x33x42ʺ()
            {
                this._ʺx25x33x42ʺ_1 = __GeneratedOdataV4.CstNodes.Inners._ʺx25x33x42ʺ.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx25x33x42ʺ _ʺx25x33x42ʺ_1 { get; }
            public static _ʺx25x33x42ʺ Instance { get; } = new _ʺx25x33x42ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
