namespace __GeneratedPartialV1.Realized.CstNodes.Rules
{
    public abstract class _alphaNumeric
    {
        private _alphaNumeric()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_alphaNumeric node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_alphaNumeric._ʺx41ʺ node, TContext context);
            protected internal abstract TResult Accept(_alphaNumeric._ʺx43ʺ node, TContext context);
        }
        
        public sealed class _ʺx41ʺ : _alphaNumeric
        {
            public _ʺx41ʺ(__GeneratedPartialV1.Realized.CstNodes.Inners._ʺx41ʺ _ʺx41ʺ_1)
            {
                this._ʺx41ʺ_1 = _ʺx41ʺ_1;
            }
            
            public __GeneratedPartialV1.Realized.CstNodes.Inners._ʺx41ʺ _ʺx41ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx43ʺ : _alphaNumeric
        {
            public _ʺx43ʺ(__GeneratedPartialV1.Realized.CstNodes.Inners._ʺx43ʺ _ʺx43ʺ_1)
            {
                this._ʺx43ʺ_1 = _ʺx43ʺ_1;
            }
            
            public __GeneratedPartialV1.Realized.CstNodes.Inners._ʺx43ʺ _ʺx43ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
