namespace __GeneratedOdataV3.CstNodes.Inners
{
    public abstract class _SPⳆHTAB
    {
        private _SPⳆHTAB()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_SPⳆHTAB node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_SPⳆHTAB._SP node, TContext context);
            protected internal abstract TResult Accept(_SPⳆHTAB._HTAB node, TContext context);
        }
        
        public sealed class _SP : _SPⳆHTAB
        {
            private _SP()
            {
                this._SP_1 = __GeneratedOdataV3.CstNodes.Rules._SP.Instance;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._SP _SP_1 { get; }
            public static _SP Instance { get; } = new _SP();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _HTAB : _SPⳆHTAB
        {
            private _HTAB()
            {
                this._HTAB_1 = __GeneratedOdataV3.CstNodes.Rules._HTAB.Instance;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._HTAB _HTAB_1 { get; }
            public static _HTAB Instance { get; } = new _HTAB();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
