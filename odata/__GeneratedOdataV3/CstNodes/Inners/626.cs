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
            public _SP(__GeneratedOdataV3.CstNodes.Rules._SP _SP_1)
            {
                this._SP_1 = _SP_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._SP _SP_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _HTAB : _SPⳆHTAB
        {
            public _HTAB(__GeneratedOdataV3.CstNodes.Rules._HTAB _HTAB_1)
            {
                this._HTAB_1 = _HTAB_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._HTAB _HTAB_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
