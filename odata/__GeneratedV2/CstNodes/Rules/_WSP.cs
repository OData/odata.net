namespace __GeneratedV2.CstNodes.Rules
{
    public abstract class _WSP
    {
        private _WSP()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_WSP node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_WSP._SP node, TContext context);
            protected internal abstract TResult Accept(_WSP._HTAB node, TContext context);
        }
        
        public sealed class _SP : _WSP
        {
            public _SP(__GeneratedV2.CstNodes.Rules._SP _SP_1)
            {
                this._SP_1 = _SP_1;
            }
            
            public __GeneratedV2.CstNodes.Rules._SP _SP_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _HTAB : _WSP
        {
            public _HTAB(__GeneratedV2.CstNodes.Rules._HTAB _HTAB_1)
            {
                this._HTAB_1 = _HTAB_1;
            }
            
            public __GeneratedV2.CstNodes.Rules._HTAB _HTAB_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
