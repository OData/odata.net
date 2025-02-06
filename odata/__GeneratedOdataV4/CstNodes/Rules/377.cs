namespace __GeneratedOdataV4.CstNodes.Rules
{
    public abstract class _SIGN
    {
        private _SIGN()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_SIGN node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_SIGN._ʺx2Bʺ node, TContext context);
            protected internal abstract TResult Accept(_SIGN._ʺx25x32x42ʺ node, TContext context);
            protected internal abstract TResult Accept(_SIGN._ʺx2Dʺ node, TContext context);
        }
        
        public sealed class _ʺx2Bʺ : _SIGN
        {
            private _ʺx2Bʺ()
            {
                this._ʺx2Bʺ_1 = __GeneratedOdataV4.CstNodes.Inners._ʺx2Bʺ.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx2Bʺ _ʺx2Bʺ_1 { get; }
            public static _ʺx2Bʺ Instance { get; } = new _ʺx2Bʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx25x32x42ʺ : _SIGN
        {
            private _ʺx25x32x42ʺ()
            {
                this._ʺx25x32x42ʺ_1 = __GeneratedOdataV4.CstNodes.Inners._ʺx25x32x42ʺ.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx25x32x42ʺ _ʺx25x32x42ʺ_1 { get; }
            public static _ʺx25x32x42ʺ Instance { get; } = new _ʺx25x32x42ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx2Dʺ : _SIGN
        {
            private _ʺx2Dʺ()
            {
                this._ʺx2Dʺ_1 = __GeneratedOdataV4.CstNodes.Inners._ʺx2Dʺ.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx2Dʺ _ʺx2Dʺ_1 { get; }
            public static _ʺx2Dʺ Instance { get; } = new _ʺx2Dʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
