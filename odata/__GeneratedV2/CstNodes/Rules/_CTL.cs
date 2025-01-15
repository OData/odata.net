namespace __GeneratedV2.CstNodes.Rules
{
    public abstract class _CTL
    {
        private _CTL()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_CTL node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_CTL._Ⰳx00ⲻ1F node, TContext context);
            protected internal abstract TResult Accept(_CTL._Ⰳx7F node, TContext context);
        }
        
        public sealed class _Ⰳx00ⲻ1F : _CTL
        {
            public _Ⰳx00ⲻ1F(__GeneratedV2.CstNodes.Inners._Ⰳx00ⲻ1F _Ⰳx00ⲻ1F_1)
            {
                this._Ⰳx00ⲻ1F_1 = _Ⰳx00ⲻ1F_1;
            }
            
            public __GeneratedV2.CstNodes.Inners._Ⰳx00ⲻ1F _Ⰳx00ⲻ1F_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _Ⰳx7F : _CTL
        {
            public _Ⰳx7F(__GeneratedV2.CstNodes.Inners._Ⰳx7F _Ⰳx7F_1)
            {
                this._Ⰳx7F_1 = _Ⰳx7F_1;
            }
            
            public __GeneratedV2.CstNodes.Inners._Ⰳx7F _Ⰳx7F_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
