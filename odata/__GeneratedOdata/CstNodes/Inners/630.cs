namespace __GeneratedOdata.CstNodes.Inners
{
    public abstract class _SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ
    {
        private _SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._SP node, TContext context);
            protected internal abstract TResult Accept(_SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._HTAB node, TContext context);
            protected internal abstract TResult Accept(_SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._ʺx25x32x30ʺ node, TContext context);
            protected internal abstract TResult Accept(_SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._ʺx25x30x39ʺ node, TContext context);
        }
        
        public sealed class _SP : _SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ
        {
            public _SP(__GeneratedOdata.CstNodes.Rules._SP _SP_1)
            {
                this._SP_1 = _SP_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._SP _SP_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _HTAB : _SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ
        {
            public _HTAB(__GeneratedOdata.CstNodes.Rules._HTAB _HTAB_1)
            {
                this._HTAB_1 = _HTAB_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._HTAB _HTAB_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx25x32x30ʺ : _SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ
        {
            public _ʺx25x32x30ʺ(__GeneratedOdata.CstNodes.Inners._ʺx25x32x30ʺ _ʺx25x32x30ʺ_1)
            {
                this._ʺx25x32x30ʺ_1 = _ʺx25x32x30ʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx25x32x30ʺ _ʺx25x32x30ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx25x30x39ʺ : _SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ
        {
            public _ʺx25x30x39ʺ(__GeneratedOdata.CstNodes.Inners._ʺx25x30x39ʺ _ʺx25x30x39ʺ_1)
            {
                this._ʺx25x30x39ʺ_1 = _ʺx25x30x39ʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx25x30x39ʺ _ʺx25x30x39ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
