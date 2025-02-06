namespace __GeneratedOdataV4.CstNodes.Rules
{
    public abstract class _AT
    {
        private _AT()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_AT node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_AT._ʺx40ʺ node, TContext context);
            protected internal abstract TResult Accept(_AT._ʺx25x34x30ʺ node, TContext context);
        }
        
        public sealed class _ʺx40ʺ : _AT
        {
            private _ʺx40ʺ()
            {
                this._ʺx40ʺ_1 = __GeneratedOdataV4.CstNodes.Inners._ʺx40ʺ.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx40ʺ _ʺx40ʺ_1 { get; }
            public static _ʺx40ʺ Instance { get; } = new _ʺx40ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx25x34x30ʺ : _AT
        {
            private _ʺx25x34x30ʺ()
            {
                this._ʺx25x34x30ʺ_1 = __GeneratedOdataV4.CstNodes.Inners._ʺx25x34x30ʺ.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx25x34x30ʺ _ʺx25x34x30ʺ_1 { get; }
            public static _ʺx25x34x30ʺ Instance { get; } = new _ʺx25x34x30ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
