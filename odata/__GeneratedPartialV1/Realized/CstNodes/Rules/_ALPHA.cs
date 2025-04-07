namespace __GeneratedPartialV1.Realized.CstNodes.Rules
{
    public abstract class _ALPHA
    {
        private _ALPHA()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ALPHA node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ALPHA._Ⰳx41ⲻ5A node, TContext context);
            protected internal abstract TResult Accept(_ALPHA._Ⰳx61ⲻ7A node, TContext context);
        }
        
        public sealed class _Ⰳx41ⲻ5A : _ALPHA
        {
            public _Ⰳx41ⲻ5A(__GeneratedPartialV1.Realized.CstNodes.Inners._Ⰳx41ⲻ5A _Ⰳx41ⲻ5A_1)
            {
                this._Ⰳx41ⲻ5A_1 = _Ⰳx41ⲻ5A_1;
            }
            
            public __GeneratedPartialV1.Realized.CstNodes.Inners._Ⰳx41ⲻ5A _Ⰳx41ⲻ5A_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _Ⰳx61ⲻ7A : _ALPHA
        {
            public _Ⰳx61ⲻ7A(__GeneratedPartialV1.Realized.CstNodes.Inners._Ⰳx61ⲻ7A _Ⰳx61ⲻ7A_1)
            {
                this._Ⰳx61ⲻ7A_1 = _Ⰳx61ⲻ7A_1;
            }
            
            public __GeneratedPartialV1.Realized.CstNodes.Inners._Ⰳx61ⲻ7A _Ⰳx61ⲻ7A_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
