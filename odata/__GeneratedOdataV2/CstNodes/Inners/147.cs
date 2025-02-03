namespace __GeneratedOdataV2.CstNodes.Inners
{
    public abstract class _ʺx61x73x63ʺⳆʺx64x65x73x63ʺ
    {
        private _ʺx61x73x63ʺⳆʺx64x65x73x63ʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ʺx61x73x63ʺⳆʺx64x65x73x63ʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ʺx61x73x63ʺⳆʺx64x65x73x63ʺ._ʺx61x73x63ʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx61x73x63ʺⳆʺx64x65x73x63ʺ._ʺx64x65x73x63ʺ node, TContext context);
        }
        
        public sealed class _ʺx61x73x63ʺ : _ʺx61x73x63ʺⳆʺx64x65x73x63ʺ
        {
            public _ʺx61x73x63ʺ(__GeneratedOdataV2.CstNodes.Inners._ʺx61x73x63ʺ _ʺx61x73x63ʺ_1)
            {
                this._ʺx61x73x63ʺ_1 = _ʺx61x73x63ʺ_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx61x73x63ʺ _ʺx61x73x63ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx64x65x73x63ʺ : _ʺx61x73x63ʺⳆʺx64x65x73x63ʺ
        {
            public _ʺx64x65x73x63ʺ(__GeneratedOdataV2.CstNodes.Inners._ʺx64x65x73x63ʺ _ʺx64x65x73x63ʺ_1)
            {
                this._ʺx64x65x73x63ʺ_1 = _ʺx64x65x73x63ʺ_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx64x65x73x63ʺ _ʺx64x65x73x63ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
