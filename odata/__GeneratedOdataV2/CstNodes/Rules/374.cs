namespace __GeneratedOdataV2.CstNodes.Rules
{
    public abstract class _COLON
    {
        private _COLON()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_COLON node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_COLON._ʺx3Aʺ node, TContext context);
            protected internal abstract TResult Accept(_COLON._ʺx25x33x41ʺ node, TContext context);
        }
        
        public sealed class _ʺx3Aʺ : _COLON
        {
            private _ʺx3Aʺ()
            {
                this._ʺx3Aʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx3Aʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx3Aʺ _ʺx3Aʺ_1 { get; }
            public static _ʺx3Aʺ Instance { get; } = new _ʺx3Aʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx25x33x41ʺ : _COLON
        {
            private _ʺx25x33x41ʺ()
            {
                this._ʺx25x33x41ʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx25x33x41ʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx25x33x41ʺ _ʺx25x33x41ʺ_1 { get; }
            public static _ʺx25x33x41ʺ Instance { get; } = new _ʺx25x33x41ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
