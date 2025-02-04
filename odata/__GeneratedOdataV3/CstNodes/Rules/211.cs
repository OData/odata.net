namespace __GeneratedOdataV3.CstNodes.Rules
{
    public abstract class _quotationⲻmark
    {
        private _quotationⲻmark()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_quotationⲻmark node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_quotationⲻmark._DQUOTE node, TContext context);
            protected internal abstract TResult Accept(_quotationⲻmark._ʺx25x32x32ʺ node, TContext context);
        }
        
        public sealed class _DQUOTE : _quotationⲻmark
        {
            private _DQUOTE()
            {
                this._DQUOTE_1 = __GeneratedOdataV3.CstNodes.Rules._DQUOTE.Instance;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._DQUOTE _DQUOTE_1 { get; }
            public static _DQUOTE Instance { get; } = new _DQUOTE();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx25x32x32ʺ : _quotationⲻmark
        {
            private _ʺx25x32x32ʺ()
            {
                this._ʺx25x32x32ʺ_1 = __GeneratedOdataV3.CstNodes.Inners._ʺx25x32x32ʺ.Instance;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx25x32x32ʺ _ʺx25x32x32ʺ_1 { get; }
            public static _ʺx25x32x32ʺ Instance { get; } = new _ʺx25x32x32ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
