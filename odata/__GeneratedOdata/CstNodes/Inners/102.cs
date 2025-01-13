namespace __GeneratedOdata.CstNodes.Inners
{
    public abstract class _ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺ
    {
        private _ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺ._ʺx24x65x78x70x61x6Ex64ʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺ._ʺx65x78x70x61x6Ex64ʺ node, TContext context);
        }
        
        public sealed class _ʺx24x65x78x70x61x6Ex64ʺ : _ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺ
        {
            public _ʺx24x65x78x70x61x6Ex64ʺ(__GeneratedOdata.CstNodes.Inners._ʺx24x65x78x70x61x6Ex64ʺ _ʺx24x65x78x70x61x6Ex64ʺ_1)
            {
                this._ʺx24x65x78x70x61x6Ex64ʺ_1 = _ʺx24x65x78x70x61x6Ex64ʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx24x65x78x70x61x6Ex64ʺ _ʺx24x65x78x70x61x6Ex64ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx65x78x70x61x6Ex64ʺ : _ʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺ
        {
            public _ʺx65x78x70x61x6Ex64ʺ(__GeneratedOdata.CstNodes.Inners._ʺx65x78x70x61x6Ex64ʺ _ʺx65x78x70x61x6Ex64ʺ_1)
            {
                this._ʺx65x78x70x61x6Ex64ʺ_1 = _ʺx65x78x70x61x6Ex64ʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx65x78x70x61x6Ex64ʺ _ʺx65x78x70x61x6Ex64ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
