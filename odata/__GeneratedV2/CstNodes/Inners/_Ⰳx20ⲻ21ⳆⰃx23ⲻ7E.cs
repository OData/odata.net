namespace __GeneratedV2.CstNodes.Inners
{
    public abstract class _Ⰳx20ⲻ21ⳆⰃx23ⲻ7E
    {
        private _Ⰳx20ⲻ21ⳆⰃx23ⲻ7E()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_Ⰳx20ⲻ21ⳆⰃx23ⲻ7E node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_Ⰳx20ⲻ21ⳆⰃx23ⲻ7E._Ⰳx20ⲻ21 node, TContext context);
            protected internal abstract TResult Accept(_Ⰳx20ⲻ21ⳆⰃx23ⲻ7E._Ⰳx23ⲻ7E node, TContext context);
        }
        
        public sealed class _Ⰳx20ⲻ21 : _Ⰳx20ⲻ21ⳆⰃx23ⲻ7E
        {
            public _Ⰳx20ⲻ21(__GeneratedV2.CstNodes.Inners._Ⰳx20ⲻ21 _Ⰳx20ⲻ21_1)
            {
                this._Ⰳx20ⲻ21_1 = _Ⰳx20ⲻ21_1;
            }
            
            public __GeneratedV2.CstNodes.Inners._Ⰳx20ⲻ21 _Ⰳx20ⲻ21_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _Ⰳx23ⲻ7E : _Ⰳx20ⲻ21ⳆⰃx23ⲻ7E
        {
            public _Ⰳx23ⲻ7E(__GeneratedV2.CstNodes.Inners._Ⰳx23ⲻ7E _Ⰳx23ⲻ7E_1)
            {
                this._Ⰳx23ⲻ7E_1 = _Ⰳx23ⲻ7E_1;
            }
            
            public __GeneratedV2.CstNodes.Inners._Ⰳx23ⲻ7E _Ⰳx23ⲻ7E_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
