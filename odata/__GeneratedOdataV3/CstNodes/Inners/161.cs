namespace __GeneratedOdataV3.CstNodes.Inners
{
    public abstract class _ʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺ
    {
        private _ʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺ._ʺx24x69x6Ex64x65x78ʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺ._ʺx69x6Ex64x65x78ʺ node, TContext context);
        }
        
        public sealed class _ʺx24x69x6Ex64x65x78ʺ : _ʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺ
        {
            private _ʺx24x69x6Ex64x65x78ʺ()
            {
                this._ʺx24x69x6Ex64x65x78ʺ_1 = __GeneratedOdataV3.CstNodes.Inners._ʺx24x69x6Ex64x65x78ʺ.Instance;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx24x69x6Ex64x65x78ʺ _ʺx24x69x6Ex64x65x78ʺ_1 { get; }
            public static _ʺx24x69x6Ex64x65x78ʺ Instance { get; } = new _ʺx24x69x6Ex64x65x78ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx69x6Ex64x65x78ʺ : _ʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺ
        {
            private _ʺx69x6Ex64x65x78ʺ()
            {
                this._ʺx69x6Ex64x65x78ʺ_1 = __GeneratedOdataV3.CstNodes.Inners._ʺx69x6Ex64x65x78ʺ.Instance;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx69x6Ex64x65x78ʺ _ʺx69x6Ex64x65x78ʺ_1 { get; }
            public static _ʺx69x6Ex64x65x78ʺ Instance { get; } = new _ʺx69x6Ex64x65x78ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
