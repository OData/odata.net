namespace __GeneratedOdataV4.CstNodes.Rules
{
    public abstract class _COMMA
    {
        private _COMMA()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_COMMA node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_COMMA._ʺx2Cʺ node, TContext context);
            protected internal abstract TResult Accept(_COMMA._ʺx25x32x43ʺ node, TContext context);
        }
        
        public sealed class _ʺx2Cʺ : _COMMA
        {
            private _ʺx2Cʺ()
            {
                this._ʺx2Cʺ_1 = __GeneratedOdataV4.CstNodes.Inners._ʺx2Cʺ.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx2Cʺ _ʺx2Cʺ_1 { get; }
            public static _ʺx2Cʺ Instance { get; } = new _ʺx2Cʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx25x32x43ʺ : _COMMA
        {
            private _ʺx25x32x43ʺ()
            {
                this._ʺx25x32x43ʺ_1 = __GeneratedOdataV4.CstNodes.Inners._ʺx25x32x43ʺ.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx25x32x43ʺ _ʺx25x32x43ʺ_1 { get; }
            public static _ʺx25x32x43ʺ Instance { get; } = new _ʺx25x32x43ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
