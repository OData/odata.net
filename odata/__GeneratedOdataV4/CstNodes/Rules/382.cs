namespace __GeneratedOdataV4.CstNodes.Rules
{
    public abstract class _CLOSE
    {
        private _CLOSE()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_CLOSE node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_CLOSE._ʺx29ʺ node, TContext context);
            protected internal abstract TResult Accept(_CLOSE._ʺx25x32x39ʺ node, TContext context);
        }
        
        public sealed class _ʺx29ʺ : _CLOSE
        {
            private _ʺx29ʺ()
            {
                this._ʺx29ʺ_1 = __GeneratedOdataV4.CstNodes.Inners._ʺx29ʺ.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx29ʺ _ʺx29ʺ_1 { get; }
            public static _ʺx29ʺ Instance { get; } = new _ʺx29ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx25x32x39ʺ : _CLOSE
        {
            private _ʺx25x32x39ʺ()
            {
                this._ʺx25x32x39ʺ_1 = __GeneratedOdataV4.CstNodes.Inners._ʺx25x32x39ʺ.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx25x32x39ʺ _ʺx25x32x39ʺ_1 { get; }
            public static _ʺx25x32x39ʺ Instance { get; } = new _ʺx25x32x39ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
