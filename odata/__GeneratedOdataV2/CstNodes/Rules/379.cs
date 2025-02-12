namespace __GeneratedOdataV2.CstNodes.Rules
{
    public abstract class _STAR
    {
        private _STAR()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_STAR node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_STAR._ʺx2Aʺ node, TContext context);
            protected internal abstract TResult Accept(_STAR._ʺx25x32x41ʺ node, TContext context);
        }
        
        public sealed class _ʺx2Aʺ : _STAR
        {
            private _ʺx2Aʺ()
            {
                this._ʺx2Aʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx2Aʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx2Aʺ _ʺx2Aʺ_1 { get; }
            public static _ʺx2Aʺ Instance { get; } = new _ʺx2Aʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx25x32x41ʺ : _STAR
        {
            private _ʺx25x32x41ʺ()
            {
                this._ʺx25x32x41ʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx25x32x41ʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx25x32x41ʺ _ʺx25x32x41ʺ_1 { get; }
            public static _ʺx25x32x41ʺ Instance { get; } = new _ʺx25x32x41ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
